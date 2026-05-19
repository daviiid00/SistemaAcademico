namespace SistemaAcademico.Interfaces
{
    using Domain.Models;

    public interface IPersonaService
    {
        Result<Persona> ObtenerPersona(string id);
        Result<List<Persona>> ObtenerTodas();
        Result<bool> EliminarPersona(string id);
        Result<bool> AdicionarPersona(Persona persona);
    }
}
