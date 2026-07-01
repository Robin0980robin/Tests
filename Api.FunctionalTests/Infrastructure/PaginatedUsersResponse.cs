using Api.Application.Features.Usuarios.GetUsuarios;

namespace Api.FunctionalTests.Infrastructure;

public sealed record PaginatedUsersResponse(
    List<UsuarioResponse> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    bool HasPreviousPage,
    bool HasNextPage
);
