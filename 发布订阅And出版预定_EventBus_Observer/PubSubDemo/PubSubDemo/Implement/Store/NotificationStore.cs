using PubSubDemo.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubDemo.Implement
{
    internal class NotificationStore:KeyedObjectStore<Notification>
    {
        public event EventHandler<NotificationEventArgs> NewNotificationOccured;
        public override void Save(Notification target)
        {
            base.Save(target);
            NewNotificationOccured?.Invoke(this,new NotificationEventArgs(target));
        }
    }
}
