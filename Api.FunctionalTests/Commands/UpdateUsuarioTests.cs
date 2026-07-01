using System.Net;
using System.Net.Http.Json;
using Api.Application.Features.Usuarios.CreateUsuario;
using Api.Application.Features.Usuarios.GetUsuarioById;
using Api.Application.Features.Usuarios.UpdateUsuario;
using Api.FunctionalTests.Infrastructure;
using Shouldly;

namespace Api.FunctionalTests.Commands;

public class UpdateUsuarioTests : ApiFunctionalTestBase
{
    [Test]
    public async Task UpdateUsuario_updates_existing_user()
    {
        var createCommand = UsuarioBogusFactory.CreateCommand();
        var createResponse = await Client.PostAsJsonAsync("/api/usuarios", createCommand);

        createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        var createdId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        createdId.ShouldNotBe(Guid.Empty);

        var updateRequest = UsuarioBogusFactory.CreateUpdateRequest();
        var updateResponse = await Client.PutAsJsonAsync(
            $"/api/usuarios/{createdId}",
            updateRequest
        );

        updateResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

        var response = await Client.GetAsync($"/api/usuarios/{createdId}");
        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<UsuarioByIdResponse>();

        payload.ShouldNotBeNull();
        payload!.Id.ShouldBe(createdId);
        payload.Nombre.ShouldBe(updateRequest.Nombre);
        payload.Apellido.ShouldBe(updateRequest.Apellido);
        payload.Email.ShouldBe(updateRequest.Email);
    }

    [Test]
    public async Task UpdateUsuario_returns_not_found_for_unknown_id()
    {
        var updateRequest = UsuarioBogusFactory.CreateUpdateRequest();
        var response = await Client.PutAsJsonAsync(
            $"/api/usuarios/{Guid.NewGuid()}",
            updateRequest
        );

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [TestCase("", "Perez", "ana.perez@example.com")]
    [TestCase("Ana", "", "ana.perez@example.com")]
    [TestCase("Ana", "Perez", "correo-invalido")]
    public async Task UpdateUsuario_rejects_invalid_payloads(
        string nombre,
        string apellido,
        string email
    )
    {
        var command = new UpdateUsuarioRequest(nombre, apellido, email);
        var response = await Client.PutAsJsonAsync($"/api/usuarios/{Guid.NewGuid()}", command);

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task UpdateUsuario_rejects_empty_guid()
    {
        var response = await Client.PutAsJsonAsync(
            "/api/usuarios/00000000-0000-0000-0000-000000000000",
            UsuarioBogusFactory.CreateUpdateRequest()
        );

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
