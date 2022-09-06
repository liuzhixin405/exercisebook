namespace VisitorNext
{
    internal class Prostgres : IRepository
    {
        public Prostgres()
        {

        }
        public void Visit(IOperation operation)
        {
            operation.VisitPostgres(this);
        }
    }
}