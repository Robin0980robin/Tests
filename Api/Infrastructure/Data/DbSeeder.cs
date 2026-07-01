using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Bogus;

namespace Api.Infrastructure.Data;

public static class DbSeeder
{
    public static void Seed(ApplicationDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Usuarios.Any())
        {
            return; // La base de datos ya contiene registros, omitir seeding
        }

        // Configurar Bogus para instanciar la entidad Usuario con Email de Vogen
        var faker = new Faker<Usuario>()
            .CustomInstantiator(f =>
            {
                var id = Guid.NewGuid();
                var nombre = f.Name.FirstName();
                var apellido = f.Name.LastName();
                
                // Generar correo realista compatible con la regla del formato
                var emailRaw = f.Internet.Email(nombre, apellido);
                var email = Email.From(emailRaw);

                return new Usuario(id, nombre, apellido, email);
            });

        var usuariosFake = faker.Generate(10);

        context.Usuarios.AddRange(usuariosFake);
        context.SaveChanges();
    }
}
