using Cosmos.Application.Services;
using Cosmos.Core.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class FilesController : ControllerBase
    {
        private readonly IFileService<FileEntity> _fileService;

        public FilesController(IFileService<FileEntity> fileService)
        {
            _fileService = fileService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(new { message = "No file uploaded." });

            try
            {
                // Save the file using the file service
                var fileEntity = await _fileService.UploadFileAsync(file.OpenReadStream(), file.FileName);

                return CreatedAtAction(nameof(DownloadFile), new { fileId = fileEntity.Id }, new { fileurl = fileEntity.Url });
            }
            catch (Exception ex)
            {
                // Log the error or handle it as needed
                Console.WriteLine($"An error occurred while uploading the file: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
            }
        }

        [HttpGet("{fileId}")]
        public async Task<IActionResult> DownloadFile(string fileId)
        {
            try
            {
                var fileStream = await _fileService.DownloadFileAsync(fileId);
                if (fileStream == null)
                {
                    return NotFound(new { message = "File not found." });
                }

                return File(fileStream, "application/octet-stream", fileId);
            }
            catch (Exception ex)
            {
                // Handle potential errors
                Console.WriteLine($"An error occurred while downloading the file: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
            }
        }

        [HttpDelete("{fileId}")]
        public async Task<IActionResult> DeleteFile(string fileId)
        {
            try
            {
                var result = await _fileService.DeleteFileAsync(fileId);
                if (!result)
                {
                    return NotFound(new { message = "File not found." });
                }

                return NoContent(); // Success with 204 No Content
            }
            catch (Exception ex)
            {
                // Handle potential errors
                Console.WriteLine($"An error occurred while deleting the file: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
            }
        }

        [HttpGet("get-all-files")]
        public async Task<IActionResult> GetFiles()
        {
            try
            {
                var files = await _fileService.GetFilesAsync();
                if (files == null || !files.Any())
                {
                    return NoContent(); // 204 No Content if no files are available
                }

                return Ok(files); // 200 OK with the list of files
            }
            catch (Exception ex)
            {
                // Handle potential errors
                Console.WriteLine($"An error occurred while retrieving files: {ex.Message}");
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal server error" });
            }
        }
    }
}
