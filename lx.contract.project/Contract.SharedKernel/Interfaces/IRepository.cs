using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contract.SharedKernel.Interfaces
{
    public interface IRepository<T>:IRepositoryBase<T> where T:class,IAggregateRoot
    {
    }
}
