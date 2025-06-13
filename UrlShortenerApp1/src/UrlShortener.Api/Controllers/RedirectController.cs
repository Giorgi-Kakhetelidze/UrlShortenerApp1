using Microsoft.AspNetCore.Mvc;
using UrlShortenerApp1.src.UrlShortener.Api.Services.Interfaces;

namespace UrlShortenerApp1.src.UrlShortener.Api.Controllers
{
    [ApiController]
    [Route("")]
    public class RedirectController : ControllerBase
    {
        private readonly IUrlService _urlService;
        private readonly IAnalyticsService _analyticsService;
        private readonly IHttpContextAccessor _contextAccessor;

        public RedirectController(IUrlService urlService, IAnalyticsService analyticsService, IHttpContextAccessor contextAccessor)
        {
            _urlService = urlService;
            _analyticsService = analyticsService;
            _contextAccessor = contextAccessor;
        }

        [HttpGet("{shortCode}")]
        public async Task<IActionResult> RedirectToOriginal(string shortCode)
        {
            var url = await _urlService.GetByShortCodeAsync(shortCode);

            if (url == null ||
                (url.ExpirationDate.HasValue && url.ExpirationDate.Value < DateTime.UtcNow) ||
                !url.IsActive)
            {
                Console.WriteLine($"Redirect failed for {shortCode}: URL not found, expired, or inactive");
                return NotFound("This link has expired, is inactive, or doesn't exist.");
            }

            // Increment click count
            await _urlService.IncrementClickCountAsync(shortCode);
            Console.WriteLine($"Incremented click count for {shortCode}");

            // Log analytics
            var request = _contextAccessor.HttpContext?.Request;
            var userAgent = request?.Headers["User-Agent"].ToString() ?? "unknown";
            var ipAddress = request?.HttpContext?.Connection?.RemoteIpAddress?.ToString() ?? "unknown";
            Console.WriteLine($"Logging click: {shortCode}, {userAgent}, {ipAddress}");

            try
            {
                await _analyticsService.LogClickAsync(shortCode, userAgent, ipAddress);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Analytics logging failed: {ex.Message}");
            }

            return Redirect(url.OriginalUrl);
        }


    }
}