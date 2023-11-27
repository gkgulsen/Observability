using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Observability.CommonShared.DTOs
{
    public record ResponseDto<T>
    {
        public T Data { get; set; } = default!;

        [JsonIgnore]
        public int StatusCode { get; set; }
        public List<String>? Errors { get; set; }

        public static ResponseDto<T> Success(int statusCode, T data)
        {
            return new ResponseDto<T>
            {
                Data = data,
                StatusCode = statusCode
            };
        }
        public static ResponseDto<T> Success(int statusCode)
        {
            return new ResponseDto<T>
            {
                StatusCode = statusCode
            };
        }
        public static ResponseDto<T> Fail(int statusCode, string error)
        {
            return new ResponseDto<T>
            {
                Errors = new List<string> { error },
                StatusCode = statusCode
            };
        }
        public static ResponseDto<T> Fail(int statusCode, List<String> errors)
        {
            return new ResponseDto<T>
            {
                Errors = errors,
                StatusCode = statusCode
            };
        }
    }
}
