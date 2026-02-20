using InvoiceManagerAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwagger()
                .AddInvoiceManagerDbContext(builder.Configuration)
                .AddFluentValidation()
                .AddAutoMapperAndServices();

var app = builder.Build();

app.UseInvoicePipeline();

app.Run();