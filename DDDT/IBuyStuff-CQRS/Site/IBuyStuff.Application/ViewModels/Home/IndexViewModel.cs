using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using IBuyStuff.Domain.Products;

namespace IBuyStuff.Application.ViewModels.Home
{
    public class IndexViewModel:ViewModelBase
    {
        public ICollection<Product> Featured { get; set; }
        public int SubscriberCount { get; set; }
    }
}
