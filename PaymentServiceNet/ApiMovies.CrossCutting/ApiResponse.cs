using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApiMovies.CrossCutting
{
    public sealed class ApiResponse<T>
    {

        public ApiResponse() {

            this.Success = true;
        }
        public bool Success { get; init; }
        public string? Message { get; init; }
        public T? Data { get; init; }
        public string? Code { get; init; } // p.ej. "VALIDATION_ERROR"
        public IDictionary<string, string[]>? Errors { get; init; } // validación
        public string? TraceId { get; init; }

        public static ApiResponse<T> Ok(T data, string? message = null) =>
            new() { Success = true, Data = data, Message = message };

        public static ApiResponse<T> Fail(string message, string? code = null,
            IDictionary<string, string[]>? errors = null, string? traceId = null) =>
            new() { Success = false, Message = message, Code = code, Errors = errors, TraceId = traceId };
    }
}
