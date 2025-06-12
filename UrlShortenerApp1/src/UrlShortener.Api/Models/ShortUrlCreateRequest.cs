namespace UrlShortenerApp1.src.UrlShortener.Api.Models
{
    public class ShortUrlCreateRequest
    {
        public string OriginalUrl { get; set; } = default!;
        public string? CustomAlias { get; set; }
        public DateTime? ExpirationDate { get; set; }
    }
}
