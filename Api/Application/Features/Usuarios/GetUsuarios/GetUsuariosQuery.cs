using MediatR;

namespace Api.Application.Features.Usuarios.GetUsuarios;

public record GetUsuariosQuery : IRequest<List<UsuarioResponse>>;
