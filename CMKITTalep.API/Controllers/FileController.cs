using Microsoft.AspNetCore.Mvc;
using CMKITTalep.Business.Interfaces;
using CMKITTalep.Entities;

namespace CMKITTalep.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IRequestService _requestService;
        private readonly IRequestResponseService _requestResponseService;

        public FileController(IRequestService requestService, IRequestResponseService requestResponseService)
        {
            _requestService = requestService;
            _requestResponseService = requestResponseService;
        }

        [HttpGet("request/{requestId}/screenshot")]
        public async Task<IActionResult> GetRequestScreenshot(int requestId)
        {
            try
            {
                var request = await _requestService.GetByIdAsync(requestId);
                if (request == null)
                {
                    return NotFound("Request not found");
                }

                // Önce base64 verisi var mı kontrol et
                if (!string.IsNullOrEmpty(request.ScreenshotBase64))
                {
                    return Ok(new
                    {
                        base64Data = request.ScreenshotBase64,
                        fileName = request.ScreenshotFileName ?? "screenshot",
                        mimeType = request.ScreenshotMimeType ?? "image/jpeg"
                    });
                }

                // Eski filePath varsa onu döndür
                if (!string.IsNullOrEmpty(request.ScreenshotFilePath))
                {
                    return Ok(new
                    {
                        filePath = request.ScreenshotFilePath,
                        fileName = request.ScreenshotFilePath.Split('/').LastOrDefault() ?? "screenshot",
                        mimeType = GetMimeTypeFromExtension(request.ScreenshotFilePath)
                    });
                }

                return NotFound("No screenshot found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving screenshot: {ex.Message}");
            }
        }

        [HttpGet("response/{responseId}/file")]
        public async Task<IActionResult> GetResponseFile(int responseId)
        {
            try
            {
                var response = await _requestResponseService.GetByIdAsync(responseId);
                if (response == null)
                {
                    return NotFound("Response not found");
                }

                // Önce base64 verisi var mı kontrol et
                if (!string.IsNullOrEmpty(response.FileBase64))
                {
                    return Ok(new
                    {
                        base64Data = response.FileBase64,
                        fileName = response.FileName ?? "file",
                        mimeType = response.FileMimeType ?? "application/octet-stream"
                    });
                }

                // Eski filePath varsa onu döndür
                if (!string.IsNullOrEmpty(response.FilePath))
                {
                    return Ok(new
                    {
                        filePath = response.FilePath,
                        fileName = response.FilePath.Split('/').LastOrDefault() ?? "file",
                        mimeType = GetMimeTypeFromExtension(response.FilePath)
                    });
                }

                return NotFound("No file found");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Error retrieving file: {ex.Message}");
            }
        }

        private string GetMimeTypeFromExtension(string filePath)
        {
            var extension = Path.GetExtension(filePath).ToLowerInvariant();
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                ".gif" => "image/gif",
                ".webp" => "image/webp",
                ".mp4" => "video/mp4",
                ".mp3" => "audio/mp3",
                ".txt" => "text/plain",
                _ => "application/octet-stream"
            };
        }
    }
}
