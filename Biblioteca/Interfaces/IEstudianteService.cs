namespace SistemaAcademico.Interfaces
{
    using Domain.Models;

    public interface IEstudianteService : IPersonaService
    {
        Result<EstudiantePregrado> ObtenerEstudiantePregrado(string id);
        Result<EstudiantePosgrado> ObtenerEstudiantePosgrado(string id);
        Result<List<Estudiante>> ObtenerEstudiantesPorPrograma(string programa);
        Result<bool> ActualizarPromedio(string idEstudiante, double nuevoPromedio);
        Result<HistoriaAcademica> ObtenerHistoria(string idEstudiante);
    }
}


