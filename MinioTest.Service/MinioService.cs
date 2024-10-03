using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Minio;
using Minio.DataModel.Args;
using Minio.Exceptions;
using MinioTest.Domain.IService;
using MinioTest.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinioTest.Service
{
    public class MinioService:IMinioService
    {
        private readonly IMinioClient _minioclient;
        private readonly IConfiguration _config;
        public MinioService(IConfiguration config,IMinioClient minioclient)
        {
            _minioclient = minioclient;
            _config = config;
        }
        public async Task<Response> ConnectionTest()
        {
            Response response = new Response();
            try
            {
                var bucketLst = await _minioclient.ListBucketsAsync();
                response.MessageCode = "00";
                response.Message = "Successfully Connected with MinIO";
                goto Result;
            }
            catch (MinioException e)
            {
                response.MessageCode = "01";
                response.Message = $"MinIO Exception: {e.Message}";
                goto Result;
            }
            catch (Exception e)
            {
                response.MessageCode = "02";
                response.Message = $"General Exception: {e.Message}";
                goto Result;
            }

        Result:
            return response;
        }
    
        public async Task<Response> GetBuckets()
        {

            Response resultDto = new Response();
            try
            {                
                var bucketLst = await _minioclient.ListBucketsAsync();
                if (bucketLst is null)
                {
                    resultDto.MessageCode ="01";
                    resultDto.Message = "There is no any buckets!";
                }                     
                else
                    resultDto.Result = bucketLst.Buckets.Select((x)=> new BucketResponseDto( x.Name,x.CreationDate));
                goto Result;
            }
            catch (Exception e)
            {
                resultDto.MessageCode = "02";
                resultDto.Message = e.Message;
                goto Result;
            }

        Result:
            return resultDto;
        }

        public async Task<Response> UploadFileAsync(string filename, string filepath)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));

            if (string.IsNullOrEmpty(filepath))
                throw new ArgumentNullException(nameof(filepath));
            var minioSettings = _config.GetSection("MinioSettings");
            var buckername = minioSettings["BucketName"];
            if(buckername == null)
                throw new ArgumentNullException(nameof(buckername));
            Response result = new Response()
            {
                MessageCode= "00",
                Message = "Success"
            };
            try
            {
                bool bucketExists = await _minioclient.BucketExistsAsync(new BucketExistsArgs().WithBucket(buckername));
                if (!bucketExists)
                {
                    result.MessageCode = "01";
                    result.Message = "bucket doesn't exist";
                    goto Result;
                }
                await _minioclient.PutObjectAsync(new PutObjectArgs()
                .WithBucket(minioSettings["BucketName"])
                .WithObject(filename)
                .WithFileName(filepath));
            }
            catch (MinioException e)
            {
                result.MessageCode = "02";
                result.Message = e.Message;
                goto Result;
            }

        Result:
            return result;
        }
        public async Task DownloadFileAsync(string filename)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException(nameof(filename));
            var filepath = Path.Combine(Path.GetTempPath(), filename);
            var minioSettings = _config.GetSection("MinioSettings");
            var bucketname = minioSettings["BucketName"];
            if (string.IsNullOrEmpty(bucketname))
                throw new InvalidOperationException("Bucket name is not configured correctly.");

            try
            {
                await _minioclient.GetObjectAsync(new GetObjectArgs()
                    .WithBucket(bucketname)
                    .WithObject(filename)
                    .WithFile(filepath));

                Console.WriteLine($"File {filename} downloaded successfully to {filepath}");
            }
            catch (MinioException ex)
            {
                Console.WriteLine($"Error downloading file: {ex.Message}");
                throw;
            }
        }


    }
}
