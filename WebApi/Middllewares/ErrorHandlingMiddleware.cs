using Application.Exceptions;
using Common.Responses;
using System.Net;
using System.Text.Json;

namespace WebApi.Middllewares
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        public ErrorHandlingMiddleware(RequestDelegate next)
        {
            _next = next;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                Error error = new();
                switch (ex)
                {
                    case CustomValidationException vex:
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        error.FriendlyErrorMessage = vex.FriendlyErrorMessage;
                        error.ErrorMessages = vex.ErrorMessages;
                        break;
                    default:
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        error.FriendlyErrorMessage = ex.Message;
                        break;
                }

                var result = JsonSerializer.Serialize(error);
                await response.WriteAsync(result);
            }
        }
    }
}
