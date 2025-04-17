using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace Framework.ApiResponse
{
    public static class ApiResponseExtensions
    {
        public static ApiResponse<T> ToApiResponse<T>(this FluentResults.Result<T> result)
        {
            var response = new ApiResponse<T>
            {
                HasError = !result.IsSuccess,
                Message = result.IsSuccess
                    ? result.Successes.FirstOrDefault()?.Message ?? string.Empty
                    : result.Errors.FirstOrDefault()?.Message ?? string.Empty,
                Errors = result.IsSuccess
                    ? result.Successes.Select(s => new Error(s.Message, "Success")).ToList()
                    : result.Errors.Select(e => new Error(e.Message, e.Metadata.ContainsKey("Code") ? e.Metadata["Code"].ToString() : "General")).ToList(),
                HttpStatusCode = result.IsSuccess ? HttpStatusCode.OK : GetStatusCodeFromErrors(result.Errors),
                Data = result.IsSuccess ? result.Value : default
            };

            return response;
        }

        public static ApiResponse ToApiResponse(this FluentResults.Result result)
        {
            var response = new ApiResponse()
            {
                HasError = !result.IsSuccess,
                Message = result.IsSuccess
                    ? result.Successes.FirstOrDefault()?.Message ?? string.Empty
                    : result.Errors.FirstOrDefault()?.Message ?? string.Empty,
                Errors = result.IsSuccess
                    ? result.Successes.Select(s => new Error(s.Message, "Success")).ToList()
                    : result.Errors.Select(e => new Error(e.Message, e.Metadata.ContainsKey("Code") ? e.Metadata["Code"].ToString() : "General")).ToList(),
                HttpStatusCode = result.IsSuccess ? HttpStatusCode.OK : GetStatusCodeFromErrors(result.Errors),
            };

            return response;
        }

        public static IActionResult ToApiResponse(this ActionContext context)
        {
            var errors = context.ModelState
                .Where(m => m.Value?.Errors.Count > 0)
                .SelectMany(p =>
                    p.Value.Errors.Select(e => new Error(e.ErrorMessage, p.Key)))
                .ToList();

            return new ApiResponse
            {
                HasError = true,
                HttpStatusCode = HttpStatusCode.BadRequest,
                Message = "One or more validation errors occurred.",
                Errors = errors
            };
        }

        private static HttpStatusCode GetStatusCodeFromErrors(List<FluentResults.IError> errors)
        {
            return errors.Any(e => e.Message.Contains("Unauthorized")) ? HttpStatusCode.Unauthorized : HttpStatusCode.BadRequest;
        }
    }
}
