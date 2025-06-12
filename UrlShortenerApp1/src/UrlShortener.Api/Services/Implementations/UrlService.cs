using Cassandra;
using UrlShortenerApp1.src.UrlShortener.Api.Models;
using UrlShortenerApp1.src.UrlShortener.Api.Services.Interfaces;

namespace UrlShortenerApp1.src.UrlShortener.Api.Services.Implementations
{
    public class UrlService : IUrlService
    {
        private readonly Cassandra.ISession _session;

        public UrlService(Cassandra.ISession session)
        {
            _session = session;
        }

        public async Task<ShortUrl?> GetByShortCodeAsync(string shortCode)
        {
            var query = "SELECT * FROM url_shortener.urls WHERE short_code = ?";
            var ps = await _session.PrepareAsync(query);
            var bound = ps.Bind(shortCode);
            var row = (await _session.ExecuteAsync(bound)).FirstOrDefault();
            if (row == null || !(row["is_active"] is bool active && active)) return null;

            return new ShortUrl
            {
                ShortCode = row["short_code"]?.ToString() ?? "",
                OriginalUrl = row["original_url"]?.ToString() ?? "",
                CreatedAt = row["created_at"] is DateTime createdAt ? createdAt : DateTime.MinValue,
                ExpirationDate = row["expiration_date"] as DateTime?,
                ClickCount = row["click_count"] is long count ? (int)count : 0,
                IsActive = row["is_active"] is bool activeValue && activeValue
            };
        }

        public async Task<ShortUrl> CreateAsync(ShortUrlCreateRequest request)
        {
            var shortCode = string.IsNullOrEmpty(request.CustomAlias)
                ? Base62Encoder.Encode(DateTimeOffset.UtcNow.ToUnixTimeMilliseconds())
                : request.CustomAlias;

            var createdAt = DateTime.UtcNow;
            var expiration = request.ExpirationDate;

            var query = "INSERT INTO urls (short_code, original_url, created_at, expiration_date, click_count, is_active) " +
                        "VALUES (?, ?, ?, ?, ?, ?)";

            await _session.ExecuteAsync(new SimpleStatement(query, shortCode, request.OriginalUrl, createdAt, expiration, 0, true));

            return new ShortUrl
            {
                ShortCode = shortCode,
                OriginalUrl = request.OriginalUrl,
                CreatedAt = createdAt,
                ExpirationDate = expiration,
                ClickCount = 0,
                IsActive = true
            };
        }

        public async Task<ShortUrl?> UpdateAsync(string shortCode, ShortUrlUpdateRequest request)
        {
            var existing = await GetByShortCodeAsync(shortCode);
            if (existing == null) return null;

            var query = "UPDATE urls SET original_url = ?, expiration_date = ? WHERE short_code = ?";
            await _session.ExecuteAsync(new SimpleStatement(query, request.OriginalUrl, request.ExpirationDate, shortCode));
            return await GetByShortCodeAsync(shortCode);
        }

        public async Task<bool> DeleteAsync(string shortCode)
        {
            var query = "DELETE FROM urls WHERE short_code = ?";
            await _session.ExecuteAsync(new SimpleStatement(query, shortCode));
            return true;
        }

        public async Task IncrementClickCountAsync(string shortCode)
        {
            var selectQuery = "SELECT click_count FROM urls WHERE short_code = ?";
            var row = await _session.ExecuteAsync(new SimpleStatement(selectQuery, shortCode));
            var result = row.FirstOrDefault();

            if (result == null) return;

            int currentCount = (int)result["click_count"];

            int newCount = currentCount + 1;

            var updateQuery = "UPDATE urls SET click_count = ? WHERE short_code = ?";
            await _session.ExecuteAsync(new SimpleStatement(updateQuery, newCount, shortCode));
        }


        public async Task<IEnumerable<ShortUrl>> GetAllAsync()
        {
            var query = "SELECT * FROM url_shortener.urls";
            var rows = await _session.ExecuteAsync(new SimpleStatement(query));

            var results = new List<ShortUrl>();

            foreach (var row in rows)
            {
                results.Add(new ShortUrl
                {
                    ShortCode = row["short_code"]?.ToString() ?? "",
                    OriginalUrl = row["original_url"]?.ToString() ?? "",
                    CreatedAt = row["created_at"] is DateTime createdAt ? createdAt : DateTime.MinValue,
                    ExpirationDate = row["expiration_date"] as DateTime?,
                    ClickCount = row["click_count"] is long count ? (int)count : 0,
                    IsActive = row["is_active"] is bool active && active
                });
            }

            return results;
        }
    }
}
