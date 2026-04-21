using HUP.BuildingBlocks.Application.Localization;
using HUP.BuildingBlocks.Domain;

namespace HUP.Tests;


public class LocalizationTestRunner
{
    private readonly ILocalizationService _localization;
    private readonly FakeCultureService _culture;

    public LocalizationTestRunner(
        ILocalizationService localization,
        ICurrentCultureService culture)
    {
        _localization = localization;
        _culture = (FakeCultureService)culture;
    }

    public void Run()
    {
        // -------------------- TEST DATA --------------------
        var json = @"{
            ""en"": ""Pending"",
            ""ar"": ""قيد الانتظار""
        }";

        // -------------------- TEST 1: English --------------------
        _culture.Language = "en";
        var resultEn = _localization.Get<string>(json);
        Console.WriteLine("EN: " + resultEn);

        // -------------------- TEST 2: Arabic --------------------
        _culture.Language = "ar";

        var resultAr = _localization.Get<string>(json);
        Console.WriteLine("AR: " + resultAr);

        // -------------------- ENUM TEST --------------------
        _culture.Language = "en";
        Console.WriteLine("ENUM EN: " + _localization.Get(RequestStatus.Pending));
    }
}

public enum RequestStatus
{
    [Localized("Pending", "قيد الانتظار")]
    Pending
}

internal class FakeCultureService : ICurrentCultureService
{
    public string Language { get; set; }
}