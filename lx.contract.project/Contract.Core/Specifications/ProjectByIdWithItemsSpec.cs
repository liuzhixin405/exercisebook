using Ardalis.Specification;
using Contract.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Core.Specifications
{
    public class ProjectByIdWithItemsSpec : Specification<Order>, ISingleResultSpecification
    {
        public ProjectByIdWithItemsSpec(string orderId)
        {
            Query
       .Where(order => order.Id == orderId)
       .Include(order => order.Items);
        }
    }
}
