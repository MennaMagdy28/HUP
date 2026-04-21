namespace HUP.BuildingBlocks.Application.Localization
{
    public interface ILocalizationService
    {
        T Get<T>(string json);
        string Get(Enum value);
    }
}