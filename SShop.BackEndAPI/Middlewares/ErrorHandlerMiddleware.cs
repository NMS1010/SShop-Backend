using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using SShop.ViewModels.Common;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Text.Json;

namespace SShop.BackEndAPI.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        { _next = next; }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = new CustomAPIResponse<NoContentAPIResponse>();
                response.StatusCode = error switch
                {
                    AccessViolationException e => (int)HttpStatusCode.Forbidden,
                    KeyNotFoundException e => (int)HttpStatusCode.NotFound,
                    SecurityTokenException e => (int)HttpStatusCode.BadRequest,
                    UnauthorizedAccessException e => (int)HttpStatusCode.Unauthorized,
                    ValidationException e => (int)HttpStatusCode.BadRequest,
                    _ => (int)(HttpStatusCode.InternalServerError),
                };
                response.IsSuccess = false;
                response.Errors = new List<string> { error.Message };
                await context.Response.WriteAsJsonAsync(response);
            }
        }
    }
}