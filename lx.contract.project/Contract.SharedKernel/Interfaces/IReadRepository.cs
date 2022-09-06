using Ardalis.Specification;
using System;
using System.Collections.Generic;
using System.Text;

namespace Contract.SharedKernel.Interfaces
{
    public interface IReadRepository<T>: IReadRepositoryBase<T> where T : class,IAggregateRoot
    {
    }
}
