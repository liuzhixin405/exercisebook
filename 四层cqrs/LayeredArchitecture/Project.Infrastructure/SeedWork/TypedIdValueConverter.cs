using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Project.Domain.SeedWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.SeedWork
{
    public class TypedIdValueConverter<TTypedIdValue>:ValueConverter<TTypedIdValue,Guid> where TTypedIdValue : TypedIdValueBase
    {
        public TypedIdValueConverter(ConverterMappingHints mappingHints = null)
            : base(id => id.Value, value => Create(value), mappingHints)
        {
        }

        private static TTypedIdValue Create(Guid id) => Activator.CreateInstance(typeof(TTypedIdValue), id) as TTypedIdValue;
    }
}
