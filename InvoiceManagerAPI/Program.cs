using InvoiceManagerAPI.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSwagger()
                .AddInvoiceManagerDbContext(builder.Configuration)
                .AddFluentValidation()
                .AddIdentity()
                .AddAutoMapperAndServices()
                .AddAuthenticationAndAuthorization();


QuestPDF.Settings.License = QuestPDF.Infrastructure.LicenseType.Community;

var app = builder.Build();

app.UseInvoicePipeline();

app.Run();