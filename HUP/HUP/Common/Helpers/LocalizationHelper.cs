using System.Reflection;
using System.Text.Json;
using HUP.Core.Enums;

namespace HUP.Common.Helpers;

public static class LocalizationHelper
{
    private const string DefaultLanguage = "ar";

    public static T Get<T>(string json, string lang)
    {
        var dict = JsonSerializer.Deserialize<Dictionary<string, T>>(json);

        if (dict.TryGetValue(lang, out var value))
            return value;
        
        return dict[DefaultLanguage];
    }
    public static string Get(Enum value, string lang)
    {
        var field = value.GetType().GetField(value.ToString());
        var attr = field?.GetCustomAttribute<LocalizedAttribute>();

        if (attr == null)
            return value.ToString();

        return lang switch
        {
            "ar" => attr.Ar,
            _ => attr.En
        };
    }
}