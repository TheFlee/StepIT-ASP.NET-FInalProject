using FluentValidation;
using FluentValidation.AspNetCore;
using InvoiceManagerAPI.Data;
using InvoiceManagerAPI.Mappings;
using InvoiceManagerAPI.Services;
using InvoiceManagerAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using System.Reflection;

namespace InvoiceManagerAPI.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddSwagger(
        this IServiceCollection services)
    {
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Version = "v1",
                Title = "InvoiceManager API",
                Description = "API to manage customers and invoices.",
                Contact = new OpenApiContact
                {
                    Name = "InvoiceManager Team",
                    Email = "support@invoicemanager.com"
                },
                License = new OpenApiLicense
                {
                    Name = "MIT License",
                    Url = new Uri("https://opensource.org/licenses/MIT")
                }
            });

            var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

            if (File.Exists(xmlPath))
                options.IncludeXmlComments(xmlPath);
        });

        return services;
    }

    public static IServiceCollection AddInvoiceManagerDbContext(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnectionString");

        services.AddDbContext<InvoiceManagerDbContext>(
            options => options.UseSqlServer(connectionString)
            );

        return services;
    }

    public static IServiceCollection AddFluentValidation(
        this IServiceCollection services)
    {
        services.AddFluentValidationAutoValidation();
        services.AddValidatorsFromAssemblyContaining<Program>();
        return services;
    }

    public static IServiceCollection AddAutoMapperAndServices(
        this IServiceCollection services)
    {
        services.AddAutoMapper(typeof(MappingProfile));

        services.AddScoped<ICustomerService, CustomerService>();
        services.AddScoped<IInvoiceService, InvoiceService>();

        return services;
    }
}