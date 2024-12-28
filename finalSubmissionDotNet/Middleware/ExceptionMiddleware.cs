using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Net;
using System.Text.Json;
using System;
using System.Threading.Tasks;

namespace finalSubmissionDotNet.Middleware
{
    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public ExceptionMiddleware(RequestDelegate next, IWebHostEnvironment webHostEnvironment)
        {
            _next = next;
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task Invoke(HttpContext httpContext)
        {

            try
            {
                await _next(httpContext);
            }
            catch (Exception exception)
            {
                var errorDetails = new
                {
                    ExceptionType = exception.GetType().ToString(),
                    ExceptionMessage = exception.Message,
                    // to be shown only in Development environment
                    InnerException = _webHostEnvironment.IsDevelopment() ? exception.InnerException?.Message : null
                };

                httpContext.Response.ContentType = "application/json";
                httpContext.Response.StatusCode = 500;

                var response = new
                {
                    Message = "An unexpected error occurred. Please try again later.",
                    Details = errorDetails
                };

                var responseJson = JsonSerializer.Serialize(response);

                await httpContext.Response.WriteAsync(responseJson);
            }
        }
    }

    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class ExceptionMiddlewareExtensions
    {
        public static IApplicationBuilder UseExceptionMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<ExceptionMiddleware>();
        }
    }
}
