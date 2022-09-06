using PubSubDemo.DataEntity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PubSubDemo.Implement
{
    /// <summary>
    /// 通知事件持久化
    /// </summary>
    internal class NotificationEventArgs:EventArgs
    {
        private Notification notification;
        public Notification Notification => notification;

        public NotificationEventArgs(Notification notification)
        {
            this.notification = notification;
        }
    }
}
