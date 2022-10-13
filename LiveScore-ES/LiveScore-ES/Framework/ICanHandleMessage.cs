namespace LiveScore_ES.Framework
{
    public interface ICanhandleMessage<in T> where T : Message
    {
        void Handle(T message);
    }
}
