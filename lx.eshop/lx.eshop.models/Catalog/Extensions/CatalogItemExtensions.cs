using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace lx.eshop.models.Catalog.Extensions
{
    public static class CatalogItemExtensions
    {
        public static void FillProductUrl(this CatalogItem item, string baseUrl)
        {
            if (item != null)
            {
                item.PictureUri = baseUrl.Replace("[0]",item.Id.ToString());
            }
        }
    }
}
