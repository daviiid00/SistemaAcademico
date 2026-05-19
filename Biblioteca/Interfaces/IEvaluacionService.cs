namespace SistemaAcademico.Interfaces
{
    using Domain.Models;

    public interface IEvaluacionService
    {
        Result<Evaluacion> ObtenerEvaluacion(string id);
        Result<List<Evaluacion>> ObtenerEvaluacionesPorEstudiante(string idEstudiante);
        Result<List<Evaluacion>> ObtenerEvaluacionesPorAsignatura(string codigoAsignatura);
        Result<bool> RegistrarEvaluacion(Evaluacion evaluacion);
        Result<double> CalcularPromedioEstudiante(string idEstudiante);
        Result<List<Evaluacion>> ObtenerTodas();
        Result<bool> RemoverEvaluacion(string idEstudiante, string codigoAsignatura);
    }
}


