namespace VisitorNext
{
    internal class Redis:IRepository
    {
        public void Visit(IOperation operation)
        {
            operation.VisitRedis(this);
        }
    }
}