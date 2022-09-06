using Abp.Configuration.Startup;
using Abp.Localization.Dictionaries;
using Abp.Localization.Dictionaries.Xml;
using Abp.Reflection.Extensions;

namespace AbpVuenetcore.Localization
{
    public static class AbpVuenetcoreLocalizationConfigurer
    {
        public static void Configure(ILocalizationConfiguration localizationConfiguration)
        {
            localizationConfiguration.Sources.Add(
                new DictionaryBasedLocalizationSource(AbpVuenetcoreConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(AbpVuenetcoreLocalizationConfigurer).GetAssembly(),
                        "AbpVuenetcore.Localization.SourceFiles"
                    )
                )
            );
        }
    }
}
