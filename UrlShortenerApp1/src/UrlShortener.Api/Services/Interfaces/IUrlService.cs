using System.Threading.Tasks;
using UrlShortenerApp1.src.UrlShortener.Api.Models;


namespace UrlShortenerApp1.src.UrlShortener.Api.Services.Interfaces
{
    public interface IUrlService
    {
        Task<ShortUrl?> GetByShortCodeAsync(string shortCode);
        Task<ShortUrl> CreateAsync(ShortUrlCreateRequest request);
        Task<ShortUrl?> UpdateAsync(string shortCode, ShortUrlUpdateRequest request);
        Task<bool> DeleteAsync(string shortCode);
        Task IncrementClickCountAsync(string shortCode);
        Task<IEnumerable<ShortUrl>> GetAllAsync();
    }
}
