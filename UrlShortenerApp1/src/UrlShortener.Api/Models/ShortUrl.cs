namespace UrlShortenerApp1.src.UrlShortener.Api.Models
{
    public class ShortUrl
    {
        public required string ShortCode { get; set; }
        public required string OriginalUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int ClickCount { get; set; }
        public bool IsActive { get; set; }
    }
}
