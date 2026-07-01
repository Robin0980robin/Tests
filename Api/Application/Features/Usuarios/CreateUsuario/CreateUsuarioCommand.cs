using MediatR;

namespace Api.Application.Features.Usuarios.CreateUsuario;

public record CreateUsuarioCommand(string Nombre, string Apellido, string Email) : IRequest<Guid>;
