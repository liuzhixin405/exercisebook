namespace SlimMessageBus.Host.Config
{
    using System;

    public abstract class AbstractConsumerBuilder<T>
    {
        public Type MessageType { get; }

        public MessageBusSettings Settings { get; }

        protected AbstractConsumerBuilder(MessageBusSettings settings)
            : this(settings, typeof(T))
        {
        }

        protected AbstractConsumerBuilder(MessageBusSettings settings, Type messageType)
        {
            MessageType = messageType;
            Settings = settings;
        }
    }
}