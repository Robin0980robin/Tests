using Api.Domain.Entities;
using Api.Domain.Repositories;
using Api.Domain.ValueObjects;
using MediatR;

namespace Api.Application.Features.Usuarios.CreateUsuario;

public class CreateUsuarioCommandHandler(IUsuarioRepository repository) 
    : IRequestHandler<CreateUsuarioCommand, Guid>
{
    private readonly IUsuarioRepository _repository = repository;

    public async Task<Guid> Handle(CreateUsuarioCommand request, CancellationToken cancellationToken)
    {
        // Vogen valida el valor automáticamente al invocar From()
        var email = Email.From(request.Email);
        
        var usuario = new Usuario(Guid.NewGuid(), request.Nombre, request.Apellido, email);

        await _repository.AddAsync(usuario, cancellationToken);
        await _repository.SaveChangesAsync(cancellationToken);

        return usuario.Id;
    }
}
