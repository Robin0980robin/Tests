using Api.Application.Common;
using Api.Infrastructure.Data;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar OpenAPI y dependencias de endpoints automáticos (Minimal APIs)
builder.Services.AddOpenApi();
builder.Services.AddEndpoints();

// 2. Configurar la persistencia de SQL Server con la integración nativa de .NET Aspire 13+
if (!builder.Environment.IsEnvironment("Testing"))
{
    builder.AddSqlServerDbContext<ApplicationDbContext>("bd");
}

// 3. Registrar MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

// 4. Registrar FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

// 5. Ejecutar base de datos y Seeds (Seeder con Bogus) en el inicio de la aplicación
if (!app.Environment.IsEnvironment("Testing"))
{
    using var scope = app.Services.CreateScope();
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbSeeder.Seed(context);
}

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// 6. Mapear automáticamente todas las Minimal APIs (Vertical Slices)
app.MapEndpoints();

app.Run();

public partial class Program { }
