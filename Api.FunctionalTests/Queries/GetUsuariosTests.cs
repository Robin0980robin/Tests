using System.Net;
using System.Net.Http.Json;
using Api.Application.Common;
using Api.Application.Features.Usuarios.GetUsuarios;
using Api.FunctionalTests.Infrastructure;
using Shouldly;

namespace Api.FunctionalTests.Queries;

public class GetUsuariosTests : ApiFunctionalTestBase
{
    [Test]
    public async Task GetUsuarios_returns_paged_results_from_database()
    {
        var response = await Client.GetAsync("/api/usuarios?page=1&pageSize=5");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<PaginatedUsersResponse>();

        payload.ShouldNotBeNull();
        payload!.Page.ShouldBe(1);
        payload.PageSize.ShouldBe(5);
        payload.Items.Count.ShouldBe(5);
        payload.TotalCount.ShouldBe(10);
        payload.TotalPages.ShouldBe(2);
        payload.HasPreviousPage.ShouldBeFalse();
        payload.HasNextPage.ShouldBeTrue();
    }

    [Test]
    public async Task GetUsuarios_returns_empty_page_when_page_is_out_of_range()
    {
        var response = await Client.GetAsync("/api/usuarios?page=3&pageSize=5");

        response.StatusCode.ShouldBe(HttpStatusCode.OK);

        var payload = await response.Content.ReadFromJsonAsync<PaginatedUsersResponse>();

        payload.ShouldNotBeNull();
        payload!.Items.ShouldBeEmpty();
        payload.Page.ShouldBe(3);
        payload.PageSize.ShouldBe(5);
        payload.TotalCount.ShouldBe(10);
        payload.TotalPages.ShouldBe(2);
        payload.HasPreviousPage.ShouldBeTrue();
        payload.HasNextPage.ShouldBeFalse();
    }

    [TestCase(0)]
    [TestCase(101)]
    public async Task GetUsuarios_rejects_invalid_page_size(int pageSize)
    {
        var response = await Client.GetAsync($"/api/usuarios?page=1&pageSize={pageSize}");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }

    [Test]
    public async Task GetUsuarios_rejects_invalid_page_number()
    {
        var response = await Client.GetAsync("/api/usuarios?page=0&pageSize=5");

        response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
    }
}
