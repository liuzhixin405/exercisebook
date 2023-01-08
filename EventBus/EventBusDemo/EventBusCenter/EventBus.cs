using EventBusCenter.Abstraction;
using System.Collections.Concurrent;
using System.Reflection;
using System.Reflection.Emit;

namespace EventBusCenter
{
    public class EventBus
    {
        public static EventBus Default => new();

        private readonly ConcurrentDictionary<Type, List<Type>> _eventAndHandlerMapping;//发布,订阅
        public EventBus()
        {
            _eventAndHandlerMapping = new ConcurrentDictionary<Type, List<Type>>();
            MapEventToHandler();
        }

        private void MapEventToHandler()
        {
            Assembly assembly = Assembly.GetEntryAssembly();
            foreach (var type in assembly.GetTypes())
            {
                if (typeof(IEventHandler).IsAssignableFrom(type)&&!type.IsInterface)
                {
                    var handlerInterface = type.GetInterface("IEventHandler`1");
                    
                    if (handlerInterface != null)
                    {
                        Type eventDataType = handlerInterface.GetGenericArguments()[0];
                        if (_eventAndHandlerMapping.ContainsKey(eventDataType))
                        {
                            List<Type> eventTypes = _eventAndHandlerMapping[eventDataType];
                            eventTypes.Add(type);
                            _eventAndHandlerMapping[eventDataType] = eventTypes;
                        }
                        else
                        {
                            var handlerTypes = new List<Type>() { type };
                            _eventAndHandlerMapping[eventDataType] = handlerTypes;
                        }
                    } 
                }
            }
        }
        
        public ValueTask Register<TEventData>(Type eventHandler)
        {
            List<Type> handlerTypes = _eventAndHandlerMapping[typeof(TEventData)];
            if (!handlerTypes.Contains(eventHandler))
            {
                handlerTypes.Add(eventHandler);
                _eventAndHandlerMapping[typeof(TEventData)] = handlerTypes;
            }
            return new();
        }

        public ValueTask UnRegister<TEventData>(Type eventHandler)
        {
            List<Type> handlerTypes = _eventAndHandlerMapping[typeof(TEventData)];
            if (handlerTypes.Contains(eventHandler))
            {
                handlerTypes.Remove(eventHandler);
                _eventAndHandlerMapping[typeof(TEventData)] = handlerTypes;
            }
            return new();
        }

        public ValueTask Trigger<TEventData>(TEventData eventData) where TEventData : IEventData
        {
            List<Type> handlers = _eventAndHandlerMapping[eventData.GetType()];
            if(handlers != null && handlers.Count > 0)
            {
                foreach (var handler in handlers)
                {
                    MethodInfo methodInfo = handler.GetMethod("HandleEvent");
                    if (methodInfo != null)
                    {
                        object obj = Activator.CreateInstance(handler);
                        if (obj != null)
                        {
                            methodInfo.Invoke(obj, new object[] {eventData});
                        }

                    }

                }
            }
            return new();
        }
    }
}