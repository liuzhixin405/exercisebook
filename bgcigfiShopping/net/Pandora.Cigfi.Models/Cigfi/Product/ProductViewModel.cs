using System;

namespace Pandora.Cigfi.Models.Cigfi.Product
{
    public class ProductViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int SoldCount { get; set; }
        public int CategoryId { get; set; }
        public string ImageUrl { get; set; }
        public string ThumbnailUrls { get; set; }
        public string Description { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Params { get; set; }
    }
}
