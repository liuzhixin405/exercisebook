using System.Globalization;
using Microsoft.Extensions.Localization;
using spot.Application.DTOs;
using spot.Application.Interfaces;

namespace spot.WebApi.Infrastructure.Services
{
    public class Translator : ITranslator
    {
        private readonly IStringLocalizer _localizer;

        public Translator(IStringLocalizerFactory localizerFactory)
        {
            var type = typeof(SharedResource); // 假设有一个用于本地化资源的类
            _localizer = localizerFactory.Create(type);
        }

        public string this[string name] => _localizer[name];

        public string GetString(string name)
        {
            return _localizer[name];
        }

        public string GetString(TranslatorMessageDto input)
        {
            return string.Format(CultureInfo.CurrentCulture, _localizer[input.Text], input.Args);
        }
    }

    // 用于本地化资源的占位符类
    public class SharedResource
    {
    }
} 