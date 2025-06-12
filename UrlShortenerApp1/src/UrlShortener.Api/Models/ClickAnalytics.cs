namespace UrlShortenerApp1.src.UrlShortener.Api.Models
{
    public class ClickAnalytics
    {
        public string ShortCode { get; set; } = default!;
        public DateTime ClickTime { get; set; }
        public string UserAgent { get; set; } = default!;
        public string IpAddress { get; set; } = default!;
    }
}
