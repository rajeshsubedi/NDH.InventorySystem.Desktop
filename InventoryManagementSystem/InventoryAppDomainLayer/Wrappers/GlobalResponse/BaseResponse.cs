using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace InventoryAppDomainLayer.Wrappers.GlobalResponse
{
    public class BaseResponse<T>
    {
        public HttpStatusCode _statusCode { get; set; }
        public string? _message { get; set; }
        public bool _success { get; set; }
        public T _data { get; set; }

        public BaseResponse()
        {
        }

        public BaseResponse(T data, HttpStatusCode statuscode, bool success, string message)
        {
            _statusCode = statuscode;
            _success = success;
            _message = message;
            _data = data;
        }
    }
}
