using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Minio.DataModel;
using MinioTest.Domain.IService;
using MinioTest.Domain.Model;
using MinioTest.Service;

namespace MinioTestApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MinioTestController : ControllerBase
    {
        private readonly IMinioService _minioservice;
        public MinioTestController(IMinioService minioservice)
        {
            _minioservice = minioservice;
        }

        [HttpGet]
        public async Task<IActionResult> TestConnection()
        {
            return Ok(await _minioservice.ConnectionTest());
        }

        [HttpGet("/Buckets")]
        public async Task<IActionResult> GetBucket()
        {
            return Ok(await _minioservice.GetBuckets());
        }

        [HttpPost("FileUpload")]
        public async Task<IActionResult> UploadFile(string filename,IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("File Doesn't Exist! ");
            var filePath = Path.GetTempFileName();
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            
            var response = await _minioservice.UploadFileAsync(filename, filePath);
            return Ok(response);
        }


        [HttpGet("FileDownload")]
        public async Task<IActionResult> DownloadFile(string filename)
        {
            var filepath = Path.Combine(Path.GetTempPath(), filename);
            await _minioservice.DownloadFileAsync(filename);
            var fileBytes = System.IO.File.ReadAllBytes(filepath);
            return File(fileBytes, "application/octet-stream", filename);
        }
    }
}
