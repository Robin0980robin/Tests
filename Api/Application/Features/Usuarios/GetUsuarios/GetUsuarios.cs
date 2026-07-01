using Api.Application.Common;
using Api.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Features.Usuarios.GetUsuarios;

// 1. DTO de Respuesta
public record UsuarioResponse(
    Guid Id,
    string Nombre,
    string Apellido,
    string Email,
    DateTime CreadoEn
);

// 2. Consulta (Query) Paginada
public record GetUsuariosQuery(int Page = 1, int PageSize = 10)
    : IRequest<PaginatedList<UsuarioResponse>>;

// 3. Validador de los parámetros de paginación
public class GetUsuariosQueryValidator : AbstractValidator<GetUsuariosQuery>
{
    public GetUsuariosQueryValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThanOrEqualTo(1)
            .WithMessage("El número de página debe ser mayor o igual a 1.");

        RuleFor(x => x.PageSize)
            .GreaterThanOrEqualTo(1)
            .WithMessage("El tamaño de página debe ser mayor o igual a 1.")
            .LessThanOrEqualTo(100)
            .WithMessage("El tamaño de página no puede exceder 100 registros.");
    }
}

// 4. Manejador (Handler)
public class GetUsuariosQueryHandler(ApplicationDbContext context)
    : IRequestHandler<GetUsuariosQuery, PaginatedList<UsuarioResponse>>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<PaginatedList<UsuarioResponse>> Handle(
        GetUsuariosQuery request,
        CancellationToken cancellationToken
    )
    {
        // Consulta diferida para paginar en la base de datos
        var queryable = _context.Usuarios.AsNoTracking();

        return await PaginatedList<UsuarioResponse>.CreateAsync(
            queryable,
            request.Page,
            request.PageSize,
            u => new UsuarioResponse(u.Id, u.Nombre, u.Apellido, u.Email.Value, u.CreadoEn),
            cancellationToken
        );
    }
}

// 5. Endpoint de Minimal API con Query String Mapping y Filtro de Validación
public class GetUsuariosEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "api/usuarios",
                async (
                    [AsParameters] GetUsuariosQuery query,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await sender.Send(query, cancellationToken);
                    return Results.Ok(result);
                }
            )
            .WithName("GetUsuarios")
            .WithTags("Usuarios")
            .WithValidation<GetUsuariosQuery>()
            .Produces<PaginatedList<UsuarioResponse>>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);
    }
}
