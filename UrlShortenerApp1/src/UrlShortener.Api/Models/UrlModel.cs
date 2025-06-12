namespace UrlShortenerApp1.src.UrlShortener.Api.Models
{
    public class UrlModel
    {
        public required string ShortCode { get; set; }
        public required string OriginalUrl { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? ExpirationDate { get; set; }
        public int ClickCount { get; set; }
        public bool IsActive { get; set; }
    }


    public class UrlAnalyticsModel
    {
        public string ShortCode { get; set; } = string.Empty;
        public DateTime ClickDate { get; set; }
        public string UserAgent { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
    }

    public class CreateUrlRequest
    {
        public string OriginalUrl { get; set; } = string.Empty;
        public DateTime? ExpirationDate { get; set; }
        public string? CustomAlias { get; set; }
    }

    public class UpdateUrlRequest
    {
        public string OriginalUrl { get; set; } = string.Empty;
        public DateTime? ExpirationDate { get; set; }
    }
}
