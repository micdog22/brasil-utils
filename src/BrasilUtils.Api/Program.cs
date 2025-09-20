
using BrasilUtils.Api.Services;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Brasil Utils API",
        Version = "v1",
        Description = "API utilitária para CPF/CNPJ, CEP, PIX e Boletos"
    });
});
builder.Services.AddControllers();
builder.Services.AddScoped<CpfCnpjService>();
builder.Services.AddScoped<CepService>();
builder.Services.AddScoped<PixPayloadService>();
builder.Services.AddScoped<BoletoService>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();
app.MapControllers();
app.Run();

public partial class Program { }
