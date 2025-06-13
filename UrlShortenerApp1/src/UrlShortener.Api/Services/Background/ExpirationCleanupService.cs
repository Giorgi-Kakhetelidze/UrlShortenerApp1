using System;
using System.Threading;
using System.Threading.Tasks;
using Cassandra;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace UrlShortenerApp1.src.UrlShortener.Api.Services.Background
{
    public class ExpirationCleanupService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly ILogger<ExpirationCleanupService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromMinutes(30);

        public ExpirationCleanupService(IServiceProvider serviceProvider, ILogger<ExpirationCleanupService> logger)
        {
            _serviceProvider = serviceProvider;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    using (var scope = _serviceProvider.CreateScope())
                    {
                        var session = scope.ServiceProvider.GetRequiredService<Cassandra.ISession>();
                        var now = DateTime.UtcNow;

                        // Select expired URLs
                        var selectQuery = "SELECT short_code, expiration_date FROM url_shortener.urls WHERE expiration_date < ? AND is_active = true ALLOW FILTERING";
                        var statement = new SimpleStatement(selectQuery, now);
                        var rows = await session.ExecuteAsync(statement);

                        foreach (var row in rows)
                        {
                            var shortCode = row["short_code"]?.ToString();
                            if (!string.IsNullOrEmpty(shortCode))
                            {
                                var updateQuery = "UPDATE url_shortener.urls SET is_active = false WHERE short_code = ?";
                                await session.ExecuteAsync(new SimpleStatement(updateQuery, shortCode));
                                _logger.LogInformation("Marked expired short code as inactive: {ShortCode}", shortCode);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during expiration cleanup.");
                }

                await Task.Delay(_cleanupInterval, stoppingToken);
            }
        }
    }
}
