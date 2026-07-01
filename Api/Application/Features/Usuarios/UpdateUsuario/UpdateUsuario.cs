using Api.Application.Common;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Api.Application.Features.Usuarios.UpdateUsuario;

public record UpdateUsuarioRequest(string Nombre, string Apellido, string Email);

public record UpdateUsuarioCommand(Guid Id, string Nombre, string Apellido, string Email)
    : IRequest<bool>;

public class UpdateUsuarioCommandValidator : AbstractValidator<UpdateUsuarioCommand>
{
    public UpdateUsuarioCommandValidator()
    {
        RuleFor(x => x.Id).NotEmpty().WithMessage("El identificador del usuario es requerido.");

        RuleFor(x => x.Nombre)
            .NotEmpty()
            .WithMessage("El nombre es requerido.")
            .MaximumLength(100)
            .WithMessage("El nombre no debe exceder 100 caracteres.");

        RuleFor(x => x.Apellido)
            .NotEmpty()
            .WithMessage("El apellido es requerido.")
            .MaximumLength(100)
            .WithMessage("El apellido no debe exceder 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("El correo electrónico es requerido.")
            .Must(email => email.Contains("@") && email.Contains("."))
            .WithMessage("El correo electrónico no tiene un formato válido.");
    }
}

public class UpdateUsuarioCommandHandler(ApplicationDbContext context)
    : IRequestHandler<UpdateUsuarioCommand, bool>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<bool> Handle(
        UpdateUsuarioCommand request,
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

        usuario.ActualizarDatos(request.Nombre, request.Apellido, Email.From(request.Email));
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}

public class UpdateUsuarioEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPut(
                "api/usuarios/{id:guid}",
                async (
                    Guid id,
                    UpdateUsuarioRequest request,
                    IValidator<UpdateUsuarioCommand> validator,
                    ISender sender,
                    CancellationToken cancellationToken
                ) =>
                {
                    if (id == Guid.Empty)
                    {
                        return Results.BadRequest();
                    }

                    var command = new UpdateUsuarioCommand(
                        id,
                        request.Nombre,
                        request.Apellido,
                        request.Email
                    );

                    var validationResult = await validator.ValidateAsync(
                        command,
                        cancellationToken
                    );
                    if (!validationResult.IsValid)
                    {
                        return Results.ValidationProblem(validationResult.ToDictionary());
                    }

                    var updated = await sender.Send(command, cancellationToken);
                    return updated ? Results.NoContent() : Results.NotFound();
                }
            )
            .WithName("UpdateUsuario")
            .WithTags("Usuarios")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .ProducesValidationProblem(StatusCodes.Status400BadRequest);
    }
}
