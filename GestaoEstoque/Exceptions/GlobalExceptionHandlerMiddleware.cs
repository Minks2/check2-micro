using GestaoEstoque.Services; // Para acessar a ValidacaoNegocioException
using System.Net;
using System.Text.Json;

namespace GestaoEstoque.Exceptions
{
    public class GlobalExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public GlobalExceptionHandlerMiddleware(RequestDelegate next)
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
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            context.Response.ContentType = "application/json";
            var response = context.Response;

            var errorResponse = new
            {
                message = exception.Message,
                details = exception.InnerException?.Message
            };

            switch (exception)
            {
                // Erro de regra de negócio (Ex: estoque insuficiente)
                case ValidacaoNegocioException ex:
                    response.StatusCode = (int)HttpStatusCode.BadRequest; // 400
                    errorResponse = new { message = ex.Message, details = (string?)null };
                    break;
                // Outros erros
                default:
                    response.StatusCode = (int)HttpStatusCode.InternalServerError; // 500
                    errorResponse = new { message = "Ocorreu um erro interno no servidor.", details = exception.Message };
                    break;
            }

            var result = JsonSerializer.Serialize(errorResponse);
            return context.Response.WriteAsync(result);
        }
    }
}