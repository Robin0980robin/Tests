using System.Net;
using System.Net.Http.Json;
using Api.Application.Features.Usuarios.CreateUsuario;
using Api.Application.Features.Usuarios.GetUsuarios;
using Api.FunctionalTests.Infrastructure;
using Shouldly;

namespace Api.FunctionalTests.Commands;

public class CreateUsuarioTests : ApiFunctionalTestBase
{
    [Test]
    public async Task CreateUsuario_persists_data_and_get_returns_new_record()
    {
        var command = UsuarioBogusFactory.CreateCommand();
        var createResponse = await Client.PostAsJsonAsync("/api/usuarios", command);

        createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        var createId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        createId.ShouldNotBe(Guid.Empty);

        var response = await Client.GetAsync("/api/usuarios?page=1&pageSize=20");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<PaginatedUsersResponse>();

        payload.ShouldNotBeNull();
        payload!.TotalCount.ShouldBe(11);
        payload.Items.Any(x => x.Email == command.Email).ShouldBeTrue();
    }

    [TestCase("", "Perez", "ana.perez@example.com")]
    [TestCase("Ana", "", "ana.perez@example.com")]
    [TestCase("Ana", "Perez", "correo-invalido")]
    public async Task CreateUsuario_rejects_invalid_payloads(
        string nombre,
        string apellido,
        string email
    )
    {
        var command = new CreateUsuarioCommand(nombre, apellido, email);
        var response = await Client.PostAsJsonAsync("/api/usuarios", command);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
