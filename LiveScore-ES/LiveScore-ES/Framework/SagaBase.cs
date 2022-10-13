namespace LiveScore_ES.Framework
{
    public abstract class SagaBase<T>
    {
        public string SagaId { get; set; }
        public T Data { get; set; }
        public bool IsComplete()
        {
            return false;
        }
    }
}
