using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinioTest.Domain.Model
{
    public  class Response
    {
        public string? MessageCode { get; set; } = "00"; 
        public string Message { get; set; } = "Success";
        public  object Result { get; set; }
    }
}
