using Microsoft.AspNetCore.Mvc;
using UrlShortenerApp1.src.UrlShortener.Api.Models;
using UrlShortenerApp1.src.UrlShortener.Api.Services.Interfaces;

namespace UrlShortenerApp1.src.UrlShortener.Api.Controllers
{
    [ApiController]
    [Route("api/urls")]
    public class UrlsController : ControllerBase
    {
        private readonly IUrlService _urlService;

        public UrlsController(IUrlService urlService)
        {
            _urlService = urlService ?? throw new ArgumentNullException(nameof(urlService));
        }

        [HttpPost]
        public async Task<IActionResult> CreateUrl([FromBody] ShortUrlCreateRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.OriginalUrl) || !Uri.TryCreate(request.OriginalUrl, UriKind.Absolute, out _))
            {
                return BadRequest(new { error = "Invalid URL format or missing request body." });
            }

            try
            {
                var result = await _urlService.CreateAsync(request);
                return CreatedAtAction(nameof(GetUrl), new { shortCode = result.ShortCode }, result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while creating the URL.", details = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllUrls()
        {
            try
            {
                var urls = await _urlService.GetAllAsync();
                return Ok(urls);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving URLs.", details = ex.Message });
            }
        }


        [HttpGet("{shortCode}")]
        public async Task<IActionResult> GetUrl([FromRoute] string shortCode)
        {
            try
            {
                var url = await _urlService.GetByShortCodeAsync(shortCode);
                if (url == null) return NotFound(new { error = "URL not found." });
                return Ok(url);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while retrieving the URL.", details = ex.Message });
            }
        }

        [HttpPut("{shortCode}")]
        public async Task<IActionResult> UpdateUrl(string shortCode, [FromBody] ShortUrlUpdateRequest request)
        {
            if (request == null)
                return BadRequest(new { error = "Request body is required." });

            try
            {
                var updated = await _urlService.UpdateAsync(shortCode, request);
                if (updated == null) return NotFound(new { error = "URL not found." });
                return Ok(updated);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while updating the URL.", details = ex.Message });
            }
        }

        [HttpDelete("{shortCode}")]
        public async Task<IActionResult> DeleteUrl(string shortCode)
        {
            try
            {
                var result = await _urlService.DeleteAsync(shortCode);
                return result ? NoContent() : NotFound(new { error = "URL not found." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { error = "An error occurred while deleting the URL.", details = ex.Message });
            }
        }
    }
}