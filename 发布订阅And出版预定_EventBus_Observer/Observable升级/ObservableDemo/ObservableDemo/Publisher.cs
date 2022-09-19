using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ObservableDemo
{
    internal class Publisher : IObservable<Messager>
    {
        private Publisher? _next;
        private bool _disposed;

        private static Publisher? s_allListeners;
        private static readonly object s_allListenersLock = new object();
        public Publisher()
        {
            var allListenerObservable = s_allListenerObservable;
            if (allListenerObservable != null)
                allListenerObservable.OnNewDiagnosticListener(this);

            // And add it to the list of all past listeners.
            _next = s_allListeners;
            s_allListeners = this;
        }
       public static IObservable<Publisher> Observable
        {
            get
            {
                if(s_allListenerObservable==null)
                    s_allListenerObservable = new AllObservable();
                return s_allListenerObservable;
            }
        }

        public IDisposable Subscribe(IObserver<Messager> observer)
        {
            return SubscribeInternal(observer);
        }
        private IDisposable SubscribeInternal(IObserver<Messager> observer)
        {
            // If we have been disposed, we silently ignore any subscriptions.
            if (_disposed)
            {
                return new Subscription() { Owner = this };
            }
            Subscription newSubscription = new Subscription()
            {
                Observer = observer,
                Owner = this,
                Next = _subscriptions
            };

            while (Interlocked.CompareExchange(ref _subscriptions, newSubscription, newSubscription.Next) != newSubscription.Next)
                newSubscription.Next = _subscriptions;
            return newSubscription;
        }
        private bool Remove(Subscription subscription)
        {

            if (_subscriptions == subscription)
            {
                _subscriptions = subscription.Next;
                return true;
            }
            else if (_subscriptions != null)
            {
                for (var cur = _subscriptions; cur.Next != null; cur = cur.Next)
                {
                    if (cur.Next == subscription)
                    {
                        cur.Next = cur.Next.Next;
                        return true;
                    }
                }
            }

            return false;

        }
        internal class Subscription : IDisposable
        {
            internal IObserver<Messager> Observer = null;
            internal Publisher Owner = null;
            internal Subscription? Next;

            public void Dispose()
            {
                while (true)
                {
                    Subscription? subscriptions = Owner?._subscriptions;
                    Subscription? newSubscriptions = Remove(subscriptions, this);

                    if(Interlocked.CompareExchange(ref Owner._subscriptions, newSubscriptions, subscriptions) == subscriptions)
                    {
                        var cur = newSubscriptions;
                        while (cur != null)
                        {

                            Debug.Assert(!(cur.Observer == Observer), "Did not remove subscription!");
                            cur = cur.Next;
                        }
                        break;
                    }
                }
            }

            private static Subscription? Remove(Subscription? subscriptions,Subscription subscription)
            {
                if(subscriptions==null)
                    return null;
                if (subscriptions.Observer == subscription.Observer)
                    return subscriptions.Next;

#if DEBUG
                // Delay a bit.  This makes it more likely that races will happen.
                for (int i = 0; i < 100; i++)
                    GC.KeepAlive("");
#endif
                return new Subscription()
                {
                    Observer = subscriptions.Observer,
                    Owner = subscriptions.Owner,
                    Next = Remove(subscriptions.Next, subscription)
                };
            }
        }
        private class AllObservable : IObservable<Publisher>
        {
            internal void OnNewDiagnosticListener(Publisher diagnosticListener)
            {
                for (var cur = _subscriptions; cur != null; cur = cur.Next)
                    cur.Subscriber.OnNext(diagnosticListener);
            }
            public IDisposable Subscribe(IObserver<Publisher> observer)
            {
                lock (s_allListenersLock)
                {
                    for (Publisher? cur = s_allListeners;cur!=null;cur=cur._next)
                    {
                        observer.OnNext(cur);
                    }
                    _subscriptions = new AllListenerSubscription(this, observer, _subscriptions);
                    return _subscriptions;
                }
            }

            private bool Remove(AllListenerSubscription subscription)
            {
                lock (s_allListenersLock)
                {
                    if (_subscriptions == subscription)
                    {
                        _subscriptions = subscription.Next;
                        return true;
                    }
                    else if (_subscriptions != null)
                    {
                        for (var cur = _subscriptions; cur.Next != null; cur = cur.Next)
                        {
                            if (cur.Next == subscription)
                            {
                                cur.Next = cur.Next.Next;
                                return true;
                            }
                        }
                    }

                    // Subscriber likely disposed multiple times
                    return false;
                }
            }
            internal class AllListenerSubscription : IDisposable
            {
                private readonly AllObservable _owner;
                internal readonly IObserver<Publisher> Subscriber;
                internal AllListenerSubscription? Next;
                internal AllListenerSubscription(AllObservable owner,IObserver<Publisher> subscriber, AllListenerSubscription? next)
                {
                    this._owner = owner;
                    this.Subscriber = subscriber;
                    this.Next = next;

                }
                public void Dispose()
                {
                    if (_owner.Remove(this))
                    {
                        Subscriber.OnCompleted();
                    }
                }
            }
            private AllListenerSubscription? _subscriptions;
           
        }
        private Subscription? _subscriptions;
        private static AllObservable? s_allListenerObservable;

        public void Write(object? value)
        {
            for (Subscription? curSubscription = _subscriptions; curSubscription != null; curSubscription = curSubscription.Next)
                curSubscription.Observer.OnNext(new Messager() { Id = Guid.NewGuid().ToString(), Content = value.ToString() });
        }
    }
}
