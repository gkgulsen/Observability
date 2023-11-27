using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Observability.CommonShared.DTOs;

namespace Observability.CommonShared
{
    public static class ExceptionMiddleware
    {
        public static void UseExceptionMiddleware(this WebApplication app)
        {
            app.UseExceptionHandler(appError =>
            {
                appError.Run(async context =>
                {
                    var exceptionFeature = context.Features.Get<IExceptionHandlerFeature>();

                    context.Response.StatusCode = 500;

                    var response = ResponseDto<string>.Fail(500, exceptionFeature!.Error.Message);

                    await context.Response.WriteAsJsonAsync(response);

                });
            });
        }   
    }
}
