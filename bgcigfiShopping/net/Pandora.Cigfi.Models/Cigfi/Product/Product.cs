using System;
using System.Collections.Generic;

namespace Pandora.Cigfi.Models.Cigfi.Product
{
    public class Product
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; } // cigfi_product.Price
        public string ImageUrl { get; set; }
        public string ThumbnailUrls { get; set; } // cigfi_product.ThumbnailUrls (JSON)
        public int SoldCount { get; set; }
        public int Stock { get; set; }
        public long? CategoryId { get; set; }
        public string Params { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
