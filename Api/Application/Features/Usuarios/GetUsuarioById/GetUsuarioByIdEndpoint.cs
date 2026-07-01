using Api.Application.Common;
using Api.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Features.Usuarios.GetUsuarioById;

public record UsuarioByIdResponse(
    Guid Id,
    string Nombre,
    string Apellido,
    string Email,
    DateTime CreadoEn
);

public record GetUsuarioByIdQuery(Guid Id) : IRequest<UsuarioByIdResponse?>;

public class GetUsuarioByIdQueryValidator : AbstractValidator<GetUsuarioByIdQuery>
{
    public GetUsuarioByIdQueryValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El identificador del usuario es requerido.");
    }
}

public class GetUsuarioByIdQueryHandler(ApplicationDbContext context)
    : IRequestHandler<GetUsuarioByIdQuery, UsuarioByIdResponse?>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<UsuarioByIdResponse?> Handle(
        GetUsuarioByIdQuery request,
        CancellationToken cancellationToken
    )
    {
        return await _context
            .Usuarios.AsNoTracking()
            .Where(usuario => usuario.Id == request.Id)
            .Select(usuario => new UsuarioByIdResponse(
                usuario.Id,
                usuario.Nombre,
                usuario.Apellido,
                usuario.Email.Value,
                usuario.CreadoEn
            ))
            .FirstOrDefaultAsync(cancellationToken);
    }
}

public class GetUsuarioByIdEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapGet(
                "api/usuarios/{id:guid}",
                async (
                    [AsParameters] GetUsuarioByIdQuery query,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    var result = await sender.Send(query, cancellationToken);
                    return result is not null ? Results.Ok(result) : Results.NotFound();
                }
            )
            .WithName("GetUsuarioById")
            .WithTags("Usuarios")
            .WithValidation<GetUsuarioByIdQuery>()
            .Produces<UsuarioByIdResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);
    }
}
