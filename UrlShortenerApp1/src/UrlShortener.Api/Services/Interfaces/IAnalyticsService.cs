using UrlShortenerApp1.src.UrlShortener.Api.Models;


namespace UrlShortenerApp1.src.UrlShortener.Api.Services.Interfaces
{
    public interface IAnalyticsService
    {
        Task LogClickAsync(string shortCode, string userAgent, string ipAddress);
        Task<IEnumerable<ClickAnalytics>> GetClicksAsync(string shortCode);
    }
}
