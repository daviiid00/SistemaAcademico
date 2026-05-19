namespace SistemaAcademico.Services
{
    using Domain.Models;
    using Interfaces;

    public class EvaluacionService : IEvaluacionService
    {
        private readonly List<Evaluacion> _evaluaciones;

        public EvaluacionService()
        {
            _evaluaciones = new List<Evaluacion>();
        }

        public Result<Evaluacion> ObtenerEvaluacion(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return Result<Evaluacion>.Fail("ID no puede estar vacío", null);

                var evaluacion = _evaluaciones.FirstOrDefault(e => e.Id == id);
                if (evaluacion == null)
                    return Result<Evaluacion>.Fail($"Evaluación con ID {id} no encontrada", null);

                return Result<Evaluacion>.Ok(evaluacion);
            }
            catch (Exception ex)
            {
                return Result<Evaluacion>.Fail($"Error al obtener evaluación: {ex.Message}", null);
            }
        }

        public Result<List<Evaluacion>> ObtenerTodas()
        {
            try
            {
                return Result<List<Evaluacion>>.Ok(_evaluaciones.ToList());
            }
            catch (Exception ex)
            {
                return Result<List<Evaluacion>>.Fail($"Error al obtener todas las evaluaciones: {ex.Message}", new List<Evaluacion>());
            }
        }

        public Result<List<Evaluacion>> ObtenerEvaluacionesPorEstudiante(string idEstudiante)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(idEstudiante))
                    return Result<List<Evaluacion>>.Fail("ID de estudiante no puede estar vacío", new List<Evaluacion>());

                var evaluaciones = _evaluaciones
                    .Where(e => e.Estudiante.Id == idEstudiante)
                    .OrderBy(e => e.Fecha)
                    .ToList();

                return Result<List<Evaluacion>>.Ok(evaluaciones);
            }
            catch (Exception ex)
            {
                return Result<List<Evaluacion>>.Fail($"Error al obtener evaluaciones por estudiante: {ex.Message}", new List<Evaluacion>());
            }
        }

        public Result<List<Evaluacion>> ObtenerEvaluacionesPorAsignatura(string codigoAsignatura)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(codigoAsignatura))
                    return Result<List<Evaluacion>>.Fail("Código de asignatura no puede estar vacío", new List<Evaluacion>());

                var evaluaciones = _evaluaciones
                    .Where(e => e.Asignatura.Codigo == codigoAsignatura)
                    .OrderBy(e => e.Fecha)
                    .ToList();

                return Result<List<Evaluacion>>.Ok(evaluaciones);
            }
            catch (Exception ex)
            {
                return Result<List<Evaluacion>>.Fail($"Error al obtener evaluaciones por asignatura: {ex.Message}", new List<Evaluacion>());
            }
        }

        public Result<bool> RegistrarEvaluacion(Evaluacion evaluacion)
        {
            try
            {
                if (evaluacion == null)
                    return Result<bool>.Fail("Evaluación no puede ser nula", false);

                if (string.IsNullOrWhiteSpace(evaluacion.Id))
                    return Result<bool>.Fail("ID de evaluación es requerido", false);

                if (_evaluaciones.Any(e => e.Id == evaluacion.Id))
                    return Result<bool>.Fail("Evaluación con este ID ya existe", false);

                _evaluaciones.Add(evaluacion);
                return Result<bool>.Ok(true, "Evaluación registrada exitosamente");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al registrar evaluación: {ex.Message}", false);
            }
        }

        public Result<bool> RemoverEvaluacion(string idEstudiante, string codigoAsignatura)
        {
            try
            {
                var evs = _evaluaciones.Where(e => e.Estudiante.Id == idEstudiante && e.Asignatura.Codigo == codigoAsignatura).ToList();
                if (!evs.Any()) return Result<bool>.Fail("Evaluación no encontrada", false);

                foreach (var ev in evs)
                {
                    _evaluaciones.Remove(ev);
                }
                return Result<bool>.Ok(true, "Evaluación removida");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al remover: {ex.Message}", false);
            }
        }

        public Result<double> CalcularPromedioEstudiante(string idEstudiante)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(idEstudiante))
                    return Result<double>.Fail("ID de estudiante no puede estar vacío", 0);

                var evaluaciones = _evaluaciones
                    .Where(e => e.Estudiante.Id == idEstudiante)
                    .ToList();

                if (evaluaciones.Count == 0)
                    return Result<double>.Ok(0, "No hay evaluaciones registradas");

                double promedio = evaluaciones.Average(e => e.NotaFinal);
                return Result<double>.Ok(promedio);
            }
            catch (Exception ex)
            {
                return Result<double>.Fail($"Error al calcular promedio: {ex.Message}", 0);
            }
        }
    }
}



