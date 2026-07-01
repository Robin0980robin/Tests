using Api.Application.Common;
using Api.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Features.Usuarios.DeleteUsuario;

public record DeleteUsuarioCommand(Guid Id) : IRequest<bool>;

public class DeleteUsuarioCommandValidator : AbstractValidator<DeleteUsuarioCommand>
{
    public DeleteUsuarioCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El identificador del usuario es requerido.");
    }
}

public class DeleteUsuarioCommandHandler(ApplicationDbContext context)
    : IRequestHandler<DeleteUsuarioCommand, bool>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<bool> Handle(
        DeleteUsuarioCommand request,
        CancellationToken cancellationToken
    )
    {
        var usuario = await _context.Usuarios.FirstOrDefaultAsync(
            usuario => usuario.Id == request.Id,
            cancellationToken
        );

        if (usuario is null)
        {
            return false;
        }

        usuario.Eliminar();
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public class DeleteUsuarioEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapDelete(
                "api/usuarios/{id:guid}",
                async (
                    Guid id,
                    IValidator<DeleteUsuarioCommand> validator,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    if (id == Guid.Empty)
                    {
                        return Results.BadRequest();
                    }

                    var command = new DeleteUsuarioCommand(id);
                    var validationResult = await validator.ValidateAsync(
                        command,
                        cancellationToken
                    );

                    if (!validationResult.IsValid)
                    {
                        return Results.ValidationProblem(validationResult.ToDictionary());
                    }

                    var deleted = await sender.Send(command, cancellationToken);
                    return deleted ? Results.NoContent() : Results.NotFound();
                }
            )
            .WithName("DeleteUsuario")
            .WithTags("Usuarios")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);
    }
}
