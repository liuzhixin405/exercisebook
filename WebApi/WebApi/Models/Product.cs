using System.Data.Entity.Core.Metadata.Edm;

namespace WebApi.Models
{
    public class Product : IEntity<int>
    {
        public int Id { get; set; }
        public string Sku { get; set; }
        public int Count { get; set; }
        public decimal Price { get; set; }
    }
}
