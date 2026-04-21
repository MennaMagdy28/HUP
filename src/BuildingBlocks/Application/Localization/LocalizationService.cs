using System.Reflection;
using System.Text.Json;
using HUP.BuildingBlocks.Domain;

namespace HUP.BuildingBlocks.Application.Localization
{
    public class LocalizationService : ILocalizationService
    {
        private const string DefaultLanguage = "ar";
        private readonly ICurrentCultureService _culture;

        public LocalizationService(ICurrentCultureService culture)
        {
            _culture = culture;
        }

        public T Get<T>(string json)
        {
            var lang = _culture.Language;

            var dict = JsonSerializer.Deserialize<Dictionary<string, T>>(json);

            if (dict.TryGetValue(lang, out var value))
                return value;

            return dict[DefaultLanguage];
        }

        public string Get(Enum value)
        {
            var lang = _culture.Language;

            var field = value.GetType().GetField(value.ToString());
            var attr = field?.GetCustomAttribute<LocalizedAttribute>();

            if (attr == null)
                return value.ToString();

            return lang == "ar" ? attr.Ar : attr.En;
        }
    }
}