using Api.Domain.Common;
using Api.Domain.ValueObjects;

namespace Api.Domain.Entities;

public class Usuario : Entity<Guid>
{
    public string Nombre { get; private set; } = null!;
    public string Apellido { get; private set; } = null!;
    public Email Email { get; private set; }
    public DateTime CreadoEn { get; private set; }

    // Constructor privado para requerimientos de ORM (Entity Framework Core)
    private Usuario() { }

    public Usuario(Guid id, string nombre, string apellido, Email email) : base(id)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(nombre));

        if (string.IsNullOrWhiteSpace(apellido))
            throw new ArgumentException("El apellido no puede estar vacío.", nameof(apellido));

        Nombre = nombre;
        Apellido = apellido;
        Email = email;
        CreadoEn = DateTime.UtcNow;
    }

    // Comportamiento del dominio para actualizar los datos del usuario
    public void ActualizarDatos(string nombre, string apellido, Email email)
    {
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ArgumentException("El nombre no puede estar vacío.", nameof(nombre));

        if (string.IsNullOrWhiteSpace(apellido))
            throw new ArgumentException("El apellido no puede estar vacío.", nameof(apellido));

        Nombre = nombre;
        Apellido = apellido;
        Email = email;
    }
}
