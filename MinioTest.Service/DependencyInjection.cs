using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Minio;
using MinioTest.Domain.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinioTest.Service
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddMinioService(this IServiceCollection services)
        {
            services.AddScoped<IMinioService, MinioService>();
            services.AddSingleton<IMinioClient>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var endpoint = config["MinioSettings:Endpoint"];
                var accessKey = config["MinioSettings:AccessKey"];
                var secretKey = config["MinioSettings:SecretKey"];
                var secure = bool.Parse(config["MinioSettings:Secure"]);

                // Initialize MinIO client
                return new MinioClient()
                    .WithEndpoint(endpoint)
                    .WithCredentials(accessKey, secretKey)
                .WithSSL(secure)
                .Build();
            });
            return services;
        }
    }
}
