using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;

namespace Contract.Core.Interfaces
{
    public interface ISpecification<T>
    {
        Expression<Func<T,bool>> Criteria { get; }
    }
}
