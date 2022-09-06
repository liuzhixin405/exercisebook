using Ardalis.Specification;
using Contract.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contract.Core.Specifications
{
    internal class IncompleteItemsSearchSpec: Specification<ToDoItem>
    {
        public IncompleteItemsSearchSpec(string searchString)
        {
            Query.Where(item => !item.IsDone && (item.Title.Contains(searchString) || item.Description.Contains(searchString)));
        }
    }
}
