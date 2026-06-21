using FluentValidation;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace SmartCita.API.Common.Exceptions
{
    public class GlobalExceptionHandler : IExceptionHandler
    {
        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var statusCode = exception switch
            {
                ValidationException => StatusCodes.Status400BadRequest,
                ArgumentException => StatusCodes.Status400BadRequest,
                KeyNotFoundException => StatusCodes.Status404NotFound,
                InvalidOperationException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            httpContext.Response.StatusCode = statusCode;

            if (exception is ValidationException validationException)
            {
                var erroresAgrupados = validationException.Errors
                    .GroupBy(e => e.PropertyName, e => e.ErrorMessage)
                    .ToDictionary(g => g.Key, g => g.ToArray());

                await httpContext.Response.WriteAsJsonAsync(new HttpValidationProblemDetails(erroresAgrupados)
                {
                    Status = statusCode,
                    Title = "Error de Validacion de Datos"
                }, cancellationToken);

                return true;
            }

            var problemDetails = new ProblemDetails
            {
                Status = statusCode,
                Title = exception.GetType().Name,
                // Seguridad: Si es error interno (500), ocultamos el detalle real al exterior.
                Detail = statusCode == 500 ? "Ocurrio un error interno en el servidor." : exception.Message
            };

            await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);

            return true;
        }
    }
}
