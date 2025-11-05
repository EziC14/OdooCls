using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OdooCls.Core.Entities
{
    public class ApiResponse<T>
    {
        public int HttpStatusCode { get; set; }  // Código HTTP (200, 400, 500, etc.)
        public int Code { get; set; }             // Código de negocio (personalizado)
        public string Message { get; set; }
        public T Data { get; set; }

        public ApiResponse(int httpStatusCode, int code, string message, T data = default)
        {
            HttpStatusCode = httpStatusCode;
            Code = code;
            Message = message;
            Data = data;
        }
    }
}
