using System.Net;
using System.Net.Http.Json;
using Api.Application.Features.Usuarios.CreateUsuario;
using Api.Application.Features.Usuarios.GetUsuarioById;
using Api.FunctionalTests.Infrastructure;
using Shouldly;

namespace Api.FunctionalTests.Commands;

public class DeleteUsuarioTests : ApiFunctionalTestBase
{
    [Test]
    public async Task DeleteUsuario_soft_deletes_existing_user()
    {
        var createResponse = await Client.PostAsJsonAsync(
            "/api/usuarios",
            UsuarioBogusFactory.CreateCommand()
        );
        createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        var createdId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        createdId.ShouldNotBe(Guid.Empty);

        var deleteResponse = await Client.DeleteAsync($"/api/usuarios/{createdId}");
        deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var getResponse = await Client.GetAsync($"/api/usuarios/{createdId}");
        getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteUsuario_returns_not_found_for_unknown_id()
    {
        var response = await Client.DeleteAsync($"/api/usuarios/{Guid.NewGuid()}");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task DeleteUsuario_rejects_empty_guid()
    {
        var response = await Client.DeleteAsync(
            "/api/usuarios/00000000-0000-0000-0000-000000000000"
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task DeleteUsuario_is_idempotent_for_already_deleted_user()
    {
        var createResponse = await Client.PostAsJsonAsync(
            "/api/usuarios",
            UsuarioBogusFactory.CreateCommand()
        );
        createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        var createdId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        createdId.ShouldNotBe(Guid.Empty);

        var firstDelete = await Client.DeleteAsync($"/api/usuarios/{createdId}");
        firstDelete.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var secondDelete = await Client.DeleteAsync($"/api/usuarios/{createdId}");
        secondDelete.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }
}
