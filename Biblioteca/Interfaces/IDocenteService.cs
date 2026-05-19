namespace SistemaAcademico.Interfaces
{
    using Domain.Models;

    public interface IDocenteService : IPersonaService
    {
        Result<Docente> ObtenerDocente(string id);
        Result<List<Docente>> ObtenerDocentesPorDepartamento(string departamento);
        Result<bool> ActivarDocente(string id);
        Result<bool> DesactivarDocente(string id);
        Result<List<Docente>> ObtenerDocentesActivos();
    }
}
