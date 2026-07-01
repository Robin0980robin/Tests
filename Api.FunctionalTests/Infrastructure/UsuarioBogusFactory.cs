using Api.Application.Features.Usuarios.CreateUsuario;
using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Bogus;

namespace Api.FunctionalTests.Infrastructure;

public static class UsuarioBogusFactory
{
    public static List<Usuario> CreateUsuarios(int count)
    {
        var faker = new Faker<Usuario>("es").CustomInstantiator(f =>
        {
            var firstName = f.Name.FirstName();
            var lastName = f.Name.LastName();
            var email = Email.From(f.Internet.Email(firstName, lastName));

            return new Usuario(Guid.NewGuid(), firstName, lastName, email);
        });

        return faker.Generate(count);
    }

    public static CreateUsuarioCommand CreateCommand()
    {
        var faker = new Faker("es");

        return new CreateUsuarioCommand(
            faker.Name.FirstName(),
            faker.Name.LastName(),
            faker.Internet.Email()
        );
    }
}
