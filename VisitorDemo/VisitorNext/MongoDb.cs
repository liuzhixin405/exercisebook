namespace VisitorNext
{
    internal class MongoDb : IRepository
    {
        public void Visit(IOperation operation)
        {
            operation.VisitMongoDb(this);
        }
    }
}