
using InvoiceManagerAPI.Middlewares;

namespace InvoiceManagerAPI.Extensions;

public static class PipelineExtensions
{
    public static WebApplication UseInvoicePipeline(
        this WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI(
                options =>
                {
                    options.SwaggerEndpoint("/swagger/v1/swagger.json", "InvoiceManager API v1");
                    options.RoutePrefix = string.Empty;
                    options.DisplayRequestDuration();
                    options.EnableFilter();
                    options.EnableDeepLinking();
                    options.EnableTryItOutByDefault();
                }
                );
        }

        app.UseMiddleware<GlobalExceptionMiddleware>();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        return app;
    }
}