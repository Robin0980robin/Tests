using Api.Domain.Repositories;
using Api.Infrastructure.Data;
using Api.Infrastructure.Data.Repositories;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

// 1. Agregar Controladores y OpenAPI
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// 2. Configurar la persistencia de SQL Server con la integración nativa de .NET Aspire 13+
builder.AddSqlServerDbContext<ApplicationDbContext>("bd");

// 3. Registrar el repositorio de usuarios
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

// 4. Registrar MediatR
builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);
});

// 5. Registrar FluentValidation
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);

var app = builder.Build();

// 6. Ejecutar base de datos y Seeds (Seeder con Bogus) en el inicio de la aplicación
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    DbSeeder.Seed(context);
}

// Configurar el pipeline HTTP
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();
