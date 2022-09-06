using MediatR;

namespace MediatRDemo.Model
{
    /// <summary>
    /// 需要广播的消息
    /// </summary>
    public class MyNotificationCommand : INotification
    {
        /// <summary>
        /// 广播的内容
        /// </summary>
        public string Message { get; set; }
    }
}
