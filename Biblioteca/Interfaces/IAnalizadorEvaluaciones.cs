namespace SistemaAcademico.Interfaces
{
    using Domain.Models;

    /// <summary>
    /// Contrato para el servicio de análisis de evaluaciones académicas.
    /// Implementado por: AnalizadorEvaluacionService.
    /// </summary>
    public interface IAnalizadorEvaluaciones
    {
        // ── Permanencia ───────────────────────────────────────────
        Result<bool>              DetectarProblemasPermanencia(Estudiante estudiante);

        // ── Filtros ───────────────────────────────────────────────
        Result<List<Evaluacion>>  ObtenerEvaluacionesPorDebajo(double nota);
        Result<double>            CalcularTendenciaPromedio(string idEstudiante);

        // ── Estadísticas globales ─────────────────────────────────
        Result<int>               ContarAprobadas();
        Result<int>               ContarReprobadas();
        Result<string>            ObtenerAsignaturaMasPerdida();
        Result<string>            ObtenerAsignaturaMejorDesempeno();
        Result<double>            CalcularPromedioGeneral();
    }
}
