using DataConsole.Model;

namespace DataConsole
{
    public interface ICatalogService {

        IQueryable<Product> ProductsOnSale();
        IQueryable<Product> Avaliable();
        IQueryable<Product> RelatedRTo(int productId);
    }
    public class CatalogService: ICatalogService
    {
        public IQueryModelDatabase QueryModelDatabase { get; set; }

        public CatalogService(IQueryModelDatabase queryModelDatabase)
        {
            QueryModelDatabase = queryModelDatabase;
        }
        public IQueryable<Product> ProductsOnSale()
        {
            return from p in this.QueryModelDatabase.Products
                   where p.IsDiscontinued == false
                   select p;
        }
        public IQueryable<Product> Avaliable()
        {
            return from p in ProductsOnSale() where p.UnitsInStock > 0 select p;
        }

        public IQueryable<Product> RelatedRTo(int productId)
        {
            var categoryid = (from pd in ProductsOnSale()
                              where pd.Id == productId
                              select pd.Category.Id).Single();
            return from p in Avaliable()
                   where p.Id != productId && p.Category.Id == categoryid
                   select p;
        }
    }
}
