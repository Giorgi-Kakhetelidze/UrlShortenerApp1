using Cassandra;
using UrlShortenerApp1.src.UrlShortener.Api.Models;
using UrlShortenerApp1.src.UrlShortener.Api.Services.Interfaces;


namespace UrlShortenerApp1.src.UrlShortener.Api.Services.Implementations
{
    public class AnalyticsService : IAnalyticsService
    {
        private readonly Cassandra.ISession _session;

        public AnalyticsService(Cassandra.ISession session)
        {
            _session = session;
        }

        public async Task LogClickAsync(string shortCode, string userAgent, string ipAddress)
        {
            var clickDate = DateTime.UtcNow;
            var query = "INSERT INTO url_analytics (short_code, click_date, user_agent, ip_address) VALUES (?, ?, ?, ?)";
            await _session.ExecuteAsync(new SimpleStatement(query, shortCode, clickDate, userAgent, ipAddress));
        }


        public async Task<IEnumerable<ClickAnalytics>> GetClicksAsync(string shortCode)
        {
            var query = "SELECT * FROM click_analytics WHERE short_code = ?";
            var result = await _session.ExecuteAsync(new SimpleStatement(query, shortCode));

            return result.Select(static row => new ClickAnalytics
            {
                ShortCode = row["short_code"]?.ToString() ?? string.Empty,
                ClickTime = row["click_time"] is DateTime clickTime ? clickTime : default,
                UserAgent = row["user_agent"]?.ToString() ?? string.Empty,
                IpAddress = row["ip_address"]?.ToString() ?? string.Empty
            });
        }
    }
}
