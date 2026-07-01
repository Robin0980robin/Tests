using System.Net;
using System.Net.Http.Json;
using Api.Application.Features.Usuarios.CreateUsuario;
using Api.Application.Features.Usuarios.GetUsuarioById;
using Api.Application.Features.Usuarios.GetUsuarios;
using Api.FunctionalTests.Infrastructure;
using Shouldly;

namespace Api.FunctionalTests.Queries;

public class GetUsuarioByIdTests : ApiFunctionalTestBase
{
    [Test]
    public async Task GetUsuarioById_returns_the_user_when_it_exists()
    {
        var usuarios = await Client.GetFromJsonAsync<PaginatedUsersResponse>(
            "/api/usuarios?page=1&pageSize=1"
        );

        usuarios.ShouldNotBeNull();
        var usuarioId = usuarios!.Items.Single().Id;

        var response = await Client.GetAsync($"/api/usuarios/{usuarioId}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<UsuarioByIdResponse>();

        payload.ShouldNotBeNull();
        payload!.Id.ShouldBe(usuarioId);
        payload.Nombre.ShouldNotBeNullOrWhiteSpace();
        payload.Apellido.ShouldNotBeNullOrWhiteSpace();
        payload.Email.ShouldContain("@");
    }

    [Test]
    public async Task GetUsuarioById_returns_not_found_for_unknown_id()
    {
        var response = await Client.GetAsync($"/api/usuarios/{Guid.NewGuid()}");

        response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
    }

    [Test]
    public async Task GetUsuarioById_rejects_empty_guid()
    {
        var response = await Client.GetAsync($"/api/usuarios/{Guid.Empty}");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetUsuarioById_works_after_creating_a_user()
    {
        var command = UsuarioBogusFactory.CreateCommand();
        var createResponse = await Client.PostAsJsonAsync("/api/usuarios", command);

        createResponse.StatusCode.ShouldBe(HttpStatusCode.Created);

        var createdId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        createdId.ShouldNotBe(Guid.Empty);

        var response = await Client.GetAsync($"/api/usuarios/{createdId}");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<UsuarioByIdResponse>();

        payload.ShouldNotBeNull();
        payload!.Id.ShouldBe(createdId);
        payload.Email.ShouldBe(command.Email);
    }
}
