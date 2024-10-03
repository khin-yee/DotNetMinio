using MinioTest.Domain.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinioTest.Domain.IService
{
    public interface IMinioService
    {
        Task<Response> ConnectionTest();
        Task<Response> GetBuckets();
        Task<Response> UploadFileAsync(string filename, string filepath);
        Task DownloadFileAsync(string filename);

    }
}
