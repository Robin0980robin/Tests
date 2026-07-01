using Api.Application.Common;
using Api.Domain.Entities;
using Api.Domain.ValueObjects;
using Api.Infrastructure.Data;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace Api.Application.Features.Usuarios.CreateUsuario;

// 1. Comando (DTO de Entrada)
public record CreateUsuarioCommand(string Nombre, string Apellido, string Email) : IRequest<Guid>;

// 2. Validador
public class CreateUsuarioCommandValidator : AbstractValidator<CreateUsuarioCommand>
{
    public CreateUsuarioCommandValidator()
    {
        RuleFor(x => x.Nombre)
            .NotEmpty().WithMessage("El nombre es requerido.")
            .MaximumLength(100).WithMessage("El nombre no debe exceder 100 caracteres.");

        RuleFor(x => x.Apellido)
            .NotEmpty().WithMessage("El apellido es requerido.")
            .MaximumLength(100).WithMessage("El apellido no debe exceder 100 caracteres.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("El correo electrónico es requerido.")
            .Must(email => email.Contains("@") && email.Contains("."))
            .WithMessage("El correo electrónico no tiene un formato válido.");
    }
}

// 3. Manejador (Handler) - consume DbContext directamente
public class CreateUsuarioCommandHandler(ApplicationDbContext context) 
    : IRequestHandler<CreateUsuarioCommand, Guid>
{
    private readonly ApplicationDbContext _context = context;

    public async Task<Guid> Handle(CreateUsuarioCommand request, CancellationToken cancellationToken)
    {
        var email = Email.From(request.Email);
        var usuario = new Usuario(Guid.NewGuid(), request.Nombre, request.Apellido, email);

        await _context.Usuarios.AddAsync(usuario, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        return usuario.Id;
    }
}

// 4. Endpoint de Minimal API
public class CreateUsuarioEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("api/usuarios", async (CreateUsuarioCommand command, ISender sender, CancellationToken cancellationToken) =>
        {
            var id = await sender.Send(command, cancellationToken);
            return Results.Created($"/api/usuarios/{id}", id);
        })
        .WithName("CreateUsuario")
        .WithTags("Usuarios")
        .WithValidation<CreateUsuarioCommand>()
        .Produces<Guid>(StatusCodes.Status201Created)
        .ProducesValidationProblem(StatusCodes.Status400BadRequest);
    }
}
