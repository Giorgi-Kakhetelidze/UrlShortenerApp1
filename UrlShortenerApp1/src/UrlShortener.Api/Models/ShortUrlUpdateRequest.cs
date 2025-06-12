namespace UrlShortenerApp1.src.UrlShortener.Api.Models
{
    public class ShortUrlUpdateRequest
    {
        public string OriginalUrl { get; set; } = default!;
        public DateTime? ExpirationDate { get; set; }
    }
}
    