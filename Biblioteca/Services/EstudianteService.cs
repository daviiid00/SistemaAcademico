namespace SistemaAcademico.Services
{
    using Domain.Models;
    using Interfaces;
    using Aspects;

    public class EstudianteService : PersonaService, IEstudianteService
    {
        private readonly Dictionary<string, HistoriaAcademica> _historias;
        private readonly List<Evaluacion> _evaluaciones;

        public EstudianteService()
        {
            _historias = new Dictionary<string, HistoriaAcademica>();
            _evaluaciones = new List<Evaluacion>();
        }

        public override Result<bool> AdicionarPersona(Persona persona)
        {
            if (persona is not Estudiante estudiante)
                return Result<bool>.Fail("La persona debe ser un Estudiante", false);

            var baseResult = base.AdicionarPersona(persona);
            if (!baseResult.Success)
                return baseResult;

            if (!_historias.ContainsKey(estudiante.Id))
            {
                _historias[estudiante.Id] = new HistoriaAcademica(estudiante.Id, DateTime.Now.Year, "1");
            }
            
            return Result<bool>.Ok(true, "Estudiante agregado con historia académica");
        }

        public Result<EstudiantePregrado> ObtenerEstudiantePregrado(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return Result<EstudiantePregrado>.Fail("ID no puede estar vacío", null);

                var estudiante = Personas.FirstOrDefault(p => p.Id == id) as EstudiantePregrado;
                if (estudiante == null)
                    return Result<EstudiantePregrado>.Fail($"Estudiante pregrado con ID {id} no encontrado", null);

                return Result<EstudiantePregrado>.Ok(estudiante);
            }
            catch (Exception ex)
            {
                return Result<EstudiantePregrado>.Fail($"Error al obtener estudiante pregrado: {ex.Message}", null);
            }
        }

        public Result<EstudiantePosgrado> ObtenerEstudiantePosgrado(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return Result<EstudiantePosgrado>.Fail("ID no puede estar vacío", null);

                var estudiante = Personas.FirstOrDefault(p => p.Id == id) as EstudiantePosgrado;
                if (estudiante == null)
                    return Result<EstudiantePosgrado>.Fail($"Estudiante posgrado con ID {id} no encontrado", null);

                return Result<EstudiantePosgrado>.Ok(estudiante);
            }
            catch (Exception ex)
            {
                return Result<EstudiantePosgrado>.Fail($"Error al obtener estudiante posgrado: {ex.Message}", null);
            }
        }

        public Result<List<Estudiante>> ObtenerEstudiantesPorPrograma(string programa)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(programa))
                    return Result<List<Estudiante>>.Fail("Programa no puede estar vacío", new List<Estudiante>());

                var estudiantes = Personas.OfType<EstudiantePregrado>()
                    .Where(e => e.ProgramaAcademico == programa)
                    .Cast<Estudiante>()
                    .ToList();

                return Result<List<Estudiante>>.Ok(estudiantes);
            }
            catch (Exception ex)
            {
                return Result<List<Estudiante>>.Fail($"Error al obtener estudiantes por programa: {ex.Message}", new List<Estudiante>());
            }
        }

        public Result<bool> ActualizarPromedio(string idEstudiante, double nuevoPromedio)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(idEstudiante))
                    return Result<bool>.Fail("ID de estudiante no puede estar vacío", false);

                var estudiante = Personas.FirstOrDefault(p => p.Id == idEstudiante) as Estudiante;
                if (estudiante == null)
                    return Result<bool>.Fail($"Estudiante con ID {idEstudiante} no encontrado", false);

                if (nuevoPromedio < 0 || nuevoPromedio > 5.0)
                    return Result<bool>.Fail("El promedio debe estar entre 0 y 5.0", false);

                estudiante.ActualizarPromedio(nuevoPromedio);
                
                if (_historias.ContainsKey(idEstudiante))
                {
                    _historias[idEstudiante].ActualizarPromedio(nuevoPromedio);
                }

                return Result<bool>.Ok(true, "Promedio actualizado exitosamente");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al actualizar promedio: {ex.Message}", false);
            }
        }

        public Result<HistoriaAcademica> ObtenerHistoria(string idEstudiante)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(idEstudiante))
                    return Result<HistoriaAcademica>.Fail("ID de estudiante no puede estar vacío", null);

                if (!_historias.ContainsKey(idEstudiante))
                    return Result<HistoriaAcademica>.Fail($"Historia académica para ID {idEstudiante} no encontrada", null);

                return Result<HistoriaAcademica>.Ok(_historias[idEstudiante]);
            }
            catch (Exception ex)
            {
                return Result<HistoriaAcademica>.Fail($"Error al obtener historia académica: {ex.Message}", null);
            }
        }

        public void AdicionarEvaluacionAHistoria(string idEstudiante, Evaluacion evaluacion)
        {
            try
            {
                if (_historias.ContainsKey(idEstudiante))
                {
                    _historias[idEstudiante].AdicionarEvaluacion(evaluacion);
                    _evaluaciones.Add(evaluacion);
                }
            }
            catch
            {
            }
        }

        public List<Evaluacion> ObtenerEvaluacionesEstudiante(string idEstudiante)
        {
            try
            {
                return _evaluaciones.Where(e => e.Estudiante.Id == idEstudiante).ToList();
            }
            catch
            {
                return new List<Evaluacion>();
            }
        }
    }
}



