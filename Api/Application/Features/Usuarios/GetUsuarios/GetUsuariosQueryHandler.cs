using Api.Domain.Repositories;
using MediatR;

namespace Api.Application.Features.Usuarios.GetUsuarios;

public class GetUsuariosQueryHandler(IUsuarioRepository repository) 
    : IRequestHandler<GetUsuariosQuery, List<UsuarioResponse>>
{
    private readonly IUsuarioRepository _repository = repository;

    public async Task<List<UsuarioResponse>> Handle(GetUsuariosQuery request, CancellationToken cancellationToken)
    {
        var usuarios = await _repository.GetAllAsync(cancellationToken);
        
        return usuarios.Select(u => new UsuarioResponse(
            u.Id,
            u.Nombre,
            u.Apellido,
            u.Email.Value,
            u.CreadoEn
        )).ToList();
    }
}
