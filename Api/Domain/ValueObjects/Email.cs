using Vogen;

namespace Api.Domain.ValueObjects;

[ValueObject<string>]
public partial struct Email
{
    private static Validation Validate(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Validation.Invalid("El correo electrónico no puede estar vacío.");
        }

        if (!value.Contains("@") || !value.Contains("."))
        {
            return Validation.Invalid("El correo electrónico no tiene un formato válido.");
        }

        return Validation.Ok;
    }
}
