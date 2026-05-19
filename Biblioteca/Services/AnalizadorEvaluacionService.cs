namespace SistemaAcademico.Services
{
    using Domain.Models;
    using Interfaces;
    using Aspects;

    /// <summary>
    /// Analiza evaluaciones para detectar patrones de riesgo, estadísticas y permanencia.
    /// Implementa IAnalizadorEvaluaciones (SRP: solo analiza, no modifica datos).
    /// </summary>
    public class AnalizadorEvaluacionService : IAnalizadorEvaluaciones
    {
        private readonly IEvaluacionService _evaluacionService;

        public AnalizadorEvaluacionService(IEvaluacionService evaluacionService)
        {
            _evaluacionService = evaluacionService
                ?? throw new ArgumentNullException(nameof(evaluacionService));
        }

        // ── Permanencia ───────────────────────────────────────────

        /// <summary>
        /// Detecta si un estudiante tiene problemas de permanencia:
        /// promedio general < 3.0 o tiene al menos una nota reprobatoria.
        /// </summary>
        public Result<bool> DetectarProblemasPermanencia(Estudiante estudiante)
        {
            try
            {
                if (estudiante == null)
                    return Result<bool>.Fail("Estudiante no puede ser nulo.", false);

                var evaluacionesResult = _evaluacionService.ObtenerEvaluacionesPorEstudiante(estudiante.Id);
                if (!evaluacionesResult.Success)
                    return Result<bool>.Fail(evaluacionesResult.Message, false);

                var evaluaciones = evaluacionesResult.Data;
                if (evaluaciones.Count == 0)
                    return Result<bool>.Ok(false, "Sin evaluaciones para analizar.");

                bool promedioEnRiesgo = estudiante.Promedio < ValidadorAcademico.NOTA_MINIMA_APROBATORIA;
                bool tieneReprobadas  = evaluaciones.Any(e => e.NotaFinal < ValidadorAcademico.NOTA_MINIMA_APROBATORIA);

                bool tieneProblemas = promedioEnRiesgo || tieneReprobadas;
                return Result<bool>.Ok(tieneProblemas);
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al detectar problemas de permanencia: {ex.Message}", false);
            }
        }

        // ── Filtros de Evaluaciones ───────────────────────────────

        /// <summary>
        /// Retorna todas las evaluaciones con nota ESTRICTAMENTE por debajo del umbral dado.
        /// CORRECCIÓN: antes retornaba siempre lista vacía.
        /// </summary>
        public Result<List<Evaluacion>> ObtenerEvaluacionesPorDebajo(double nota)
        {
            try
            {
                var notaResult = ValidadorAcademico.ValidarNota(nota);
                if (!notaResult.valido)
                    return Result<List<Evaluacion>>.Fail(notaResult.mensaje, new List<Evaluacion>());

                var todasResult = _evaluacionService.ObtenerTodas();
                if (!todasResult.Success)
                    return Result<List<Evaluacion>>.Fail(todasResult.Message, new List<Evaluacion>());

                var filtradas = todasResult.Data
                    .Where(e => e.NotaFinal < nota)
                    .OrderBy(e => e.NotaFinal)
                    .ToList();

                return Result<List<Evaluacion>>.Ok(filtradas,
                    $"{filtradas.Count} evaluación(es) por debajo de {nota:F1}.");
            }
            catch (Exception ex)
            {
                return Result<List<Evaluacion>>.Fail(
                    $"Error al obtener evaluaciones por debajo: {ex.Message}", new List<Evaluacion>());
            }
        }

        // ── Tendencia ─────────────────────────────────────────────

        /// <summary>
        /// Calcula la tendencia del promedio de un estudiante comparando primera y segunda mitad de evaluaciones.
        /// Positivo = mejora, negativo = deterioro.
        /// </summary>
        public Result<double> CalcularTendenciaPromedio(string idEstudiante)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(idEstudiante))
                    return Result<double>.Fail("ID de estudiante no puede estar vacío.", 0);

                var evaluacionesResult = _evaluacionService.ObtenerEvaluacionesPorEstudiante(idEstudiante);
                if (!evaluacionesResult.Success)
                    return Result<double>.Fail(evaluacionesResult.Message, 0);

                var evaluaciones = evaluacionesResult.Data.OrderBy(e => e.Fecha).ToList();
                if (evaluaciones.Count < 2)
                    return Result<double>.Ok(0, "Se requieren al menos 2 evaluaciones para calcular tendencia.");

                double primerasMitad = evaluaciones.Take(evaluaciones.Count / 2).Average(e => e.NotaFinal);
                double segundaMitad  = evaluaciones.Skip(evaluaciones.Count / 2).Average(e => e.NotaFinal);
                double tendencia     = segundaMitad - primerasMitad;

                return Result<double>.Ok(Math.Round(tendencia, 2));
            }
            catch (Exception ex)
            {
                return Result<double>.Fail($"Error al calcular tendencia: {ex.Message}", 0);
            }
        }

        // ── Estadísticas Globales ─────────────────────────────────

        /// <summary>
        /// Cuenta cuántas evaluaciones tienen nota >= 3.0 (aprobadas) en todo el sistema.
        /// </summary>
        public Result<int> ContarAprobadas()
        {
            try
            {
                var todasResult = _evaluacionService.ObtenerTodas();
                if (!todasResult.Success)
                    return Result<int>.Fail(todasResult.Message, 0);

                int aprobadas = todasResult.Data
                    .Count(e => e.NotaFinal >= ValidadorAcademico.NOTA_MINIMA_APROBATORIA);

                return Result<int>.Ok(aprobadas);
            }
            catch (Exception ex)
            {
                return Result<int>.Fail($"Error al contar aprobadas: {ex.Message}", 0);
            }
        }

        /// <summary>
        /// Cuenta cuántas evaluaciones tienen nota < 3.0 (reprobadas) en todo el sistema.
        /// </summary>
        public Result<int> ContarReprobadas()
        {
            try
            {
                var todasResult = _evaluacionService.ObtenerTodas();
                if (!todasResult.Success)
                    return Result<int>.Fail(todasResult.Message, 0);

                int reprobadas = todasResult.Data
                    .Count(e => e.NotaFinal < ValidadorAcademico.NOTA_MINIMA_APROBATORIA);

                return Result<int>.Ok(reprobadas);
            }
            catch (Exception ex)
            {
                return Result<int>.Fail($"Error al contar reprobadas: {ex.Message}", 0);
            }
        }

        /// <summary>
        /// Retorna el nombre de la asignatura con mayor cantidad de notas reprobatorias (< 3.0).
        /// Si hay empate, retorna la primera en orden alfabético.
        /// </summary>
        public Result<string> ObtenerAsignaturaMasPerdida()
        {
            try
            {
                var todasResult = _evaluacionService.ObtenerTodas();
                if (!todasResult.Success)
                    return Result<string>.Fail(todasResult.Message, "N/A");

                var reprobadas = todasResult.Data
                    .Where(e => e.NotaFinal < ValidadorAcademico.NOTA_MINIMA_APROBATORIA)
                    .ToList();

                if (!reprobadas.Any())
                    return Result<string>.Ok("Ninguna", "No hay evaluaciones reprobadas.");

                var masPerdida = reprobadas
                    .GroupBy(e => e.Asignatura.Nombre)
                    .OrderByDescending(g => g.Count())
                    .ThenBy(g => g.Key)
                    .First();

                return Result<string>.Ok(
                    $"{masPerdida.Key} ({masPerdida.Count()} reprobado(s))");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Error al obtener asignatura más perdida: {ex.Message}", "N/A");
            }
        }

        /// <summary>
        /// Retorna el nombre de la asignatura con mejor promedio de notas.
        /// </summary>
        public Result<string> ObtenerAsignaturaMejorDesempeno()
        {
            try
            {
                var todasResult = _evaluacionService.ObtenerTodas();
                if (!todasResult.Success)
                    return Result<string>.Fail(todasResult.Message, "N/A");

                if (!todasResult.Data.Any())
                    return Result<string>.Ok("N/A", "No hay evaluaciones registradas.");

                var mejor = todasResult.Data
                    .GroupBy(e => e.Asignatura.Nombre)
                    .Select(g => new { Nombre = g.Key, Promedio = g.Average(e => e.NotaFinal) })
                    .OrderByDescending(x => x.Promedio)
                    .First();

                return Result<string>.Ok($"{mejor.Nombre} ({mejor.Promedio:F2})");
            }
            catch (Exception ex)
            {
                return Result<string>.Fail($"Error al obtener asignatura de mejor desempeño: {ex.Message}", "N/A");
            }
        }

        /// <summary>
        /// Retorna el promedio general de TODAS las evaluaciones del sistema.
        /// </summary>
        public Result<double> CalcularPromedioGeneral()
        {
            try
            {
                var todasResult = _evaluacionService.ObtenerTodas();
                if (!todasResult.Success)
                    return Result<double>.Fail(todasResult.Message, 0);

                if (!todasResult.Data.Any())
                    return Result<double>.Ok(0, "No hay evaluaciones registradas.");

                double promedio = todasResult.Data.Average(e => e.NotaFinal);
                return Result<double>.Ok(Math.Round(promedio, 2));
            }
            catch (Exception ex)
            {
                return Result<double>.Fail($"Error al calcular promedio general: {ex.Message}", 0);
            }
        }
    }
}
