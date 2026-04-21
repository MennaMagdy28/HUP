namespace HUP.BuildingBlocks.Application.Localization;

public class CurrentCultureService : ICurrentCultureService
{
    private readonly IHttpContextAccessor _http;

    public CurrentCultureService(IHttpContextAccessor http)
    {
        _http = http;
    }

    public string Language =>
        _http.HttpContext?.Request.Headers["Accept-Language"].ToString()
        ?? "ar";
}