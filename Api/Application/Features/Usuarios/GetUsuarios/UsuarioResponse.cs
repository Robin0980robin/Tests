namespace Api.Application.Features.Usuarios.GetUsuarios;

public record UsuarioResponse(Guid Id, string Nombre, string Apellido, string Email, DateTime CreadoEn);
