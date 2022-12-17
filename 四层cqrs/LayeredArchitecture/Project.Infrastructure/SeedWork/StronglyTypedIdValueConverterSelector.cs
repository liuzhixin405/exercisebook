using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Project.Domain.SeedWork;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project.Infrastructure.SeedWork
{
    public class StronglyTypedIdValueConverterSelector:ValueConverterSelector
    {
        private readonly ConcurrentDictionary<(Type ModelClrType, Type ProviderClrType), ValueConverterInfo> _converters
               = new ConcurrentDictionary<(Type ModelClrType, Type ProviderClrType), ValueConverterInfo>();

        public StronglyTypedIdValueConverterSelector(ValueConverterSelectorDependencies dependencies)
            : base(dependencies)
        {
        }

        public override IEnumerable<ValueConverterInfo> Select(Type modelClrType, Type providerClrType = null)
        {
            var baseConverters = base.Select(modelClrType, providerClrType);
            foreach (var converter in baseConverters)
            {
                yield return converter;
            }

            var underlyingModelType = UnwrapNullableType(modelClrType);
            var underlyingProviderType = UnwrapNullableType(providerClrType);

            if (underlyingProviderType is null || underlyingProviderType == typeof(Guid))
            {
                var isTypedIdValue = typeof(TypedIdValueBase).IsAssignableFrom(underlyingModelType);
                if (isTypedIdValue)
                {
                    var converterType = typeof(TypedIdValueConverter<>).MakeGenericType(underlyingModelType);

                    yield return _converters.GetOrAdd((underlyingModelType, typeof(Guid)), _ =>
                    {
                        return new ValueConverterInfo(
                            modelClrType: modelClrType,
                            providerClrType: typeof(Guid),
                            factory: valueConverterInfo => (ValueConverter)Activator.CreateInstance(converterType, valueConverterInfo.MappingHints));
                    });
                }
            }
        }

        private static Type UnwrapNullableType(Type type)
        {
            if (type is null)
            {
                return null;
            }

            return Nullable.GetUnderlyingType(type) ?? type;
        }
    }
}
