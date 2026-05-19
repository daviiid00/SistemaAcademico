namespace SistemaAcademico.Interfaces
{
    using Domain.Models;

    /// <summary>
    /// Contrato para las operaciones académicas de alto nivel.
    /// Implementado por: UniversidadService.
    /// </summary>
    public interface IGestionAcademica
    {
        Result<bool>                     RegistrarEvaluacion(Evaluacion evaluacion);
        Result<bool>                     CancelarMateria(CancelacionMateria cancelacion);
        Result<bool>                     GraduarEstudiante(string idEstudiante);
        Result<bool>                     EliminarEvaluacion(string id);
        Result<List<Estudiante>>         ObtenerEstudiantesEnRiesgo();
        Result<List<EventoAcademicoBase>> AnalizarEvaluacionesConEventos();
    }
}
