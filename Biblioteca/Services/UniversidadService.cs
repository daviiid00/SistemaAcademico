namespace SistemaAcademico.Services
{
    using Domain.Models;
    using Interfaces;
    using Aspects;

    /// <summary>
    /// Servicio orquestador de operaciones académicas de alto nivel.
    /// Implementa IGestionAcademica (Facade del dominio).
    /// Coordina: EstudianteService, EvaluacionService, IGestorEventos, IAnalizadorEvaluaciones.
    /// </summary>
    public class UniversidadService : IGestionAcademica
    {
        private readonly EstudianteService       _estudianteService;
        private readonly EvaluacionService       _evaluacionService;
        private readonly IGestorEventos          _gestorEventos;
        private readonly IAnalizadorEvaluaciones _analizador;

        public UniversidadService(
            EstudianteService       estudianteService,
            EvaluacionService       evaluacionService,
            IGestorEventos          gestorEventos,
            IAnalizadorEvaluaciones analizador)
        {
            _estudianteService = estudianteService ?? throw new ArgumentNullException(nameof(estudianteService));
            _evaluacionService = evaluacionService ?? throw new ArgumentNullException(nameof(evaluacionService));
            _gestorEventos     = gestorEventos     ?? throw new ArgumentNullException(nameof(gestorEventos));
            _analizador        = analizador        ?? throw new ArgumentNullException(nameof(analizador));
        }

        // ── Registro de Evaluación ────────────────────────────────

        /// <summary>
        /// Registra una evaluación, actualiza el promedio del estudiante y publica el evento correspondiente.
        /// </summary>
        public Result<bool> RegistrarEvaluacion(Evaluacion evaluacion)
        {
            try
            {
                if (evaluacion == null)
                    return Result<bool>.Fail("Evaluación no puede ser nula.", false);

                var registroResult = _evaluacionService.RegistrarEvaluacion(evaluacion);
                if (!registroResult.Success)
                    return Result<bool>.Fail(registroResult.Message, false);

                _estudianteService.AdicionarEvaluacionAHistoria(evaluacion.Estudiante.Id, evaluacion);

                var promedioResult = _evaluacionService.CalcularPromedioEstudiante(evaluacion.Estudiante.Id);
                if (promedioResult.Success)
                {
                    var updateResult = _estudianteService.ActualizarPromedio(evaluacion.Estudiante.Id, promedioResult.Data);
                    if (!updateResult.Success)
                        return Result<bool>.Fail($"Error actualizando promedio: {updateResult.Message}", false);
                }

                var evento = new EventoEvaluacionRegistrada(
                    Guid.NewGuid().ToString(),
                    DateTime.Now,
                    evaluacion.Estudiante.Id,
                    $"Evaluación registrada en {evaluacion.Asignatura.Codigo}",
                    evaluacion.Id,
                    evaluacion.NotaFinal,
                    evaluacion.Asignatura.Codigo
                );

                _gestorEventos.PublicarEvento(evento);
                return Result<bool>.Ok(true, "Evaluación registrada y evento publicado.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al registrar evaluación: {ex.Message}", false);
            }
        }

        public Result<bool> EliminarEvaluacion(string id)
        {
            try
            {
                var evalRes = _evaluacionService.ObtenerEvaluacion(id);
                if (!evalRes.Success || evalRes.Data == null)
                    return Result<bool>.Fail("Evaluación no encontrada.", false);

                var evaluacion = evalRes.Data;
                var idEstudiante = evaluacion.Estudiante.Id;

                // 1. Eliminar de EvaluacionService
                var delRes = _evaluacionService.EliminarEvaluacion(id);
                if (!delRes.Success) return delRes;

                // 2. Eliminar de EstudianteService
                _estudianteService.RemoverEvaluacionDeHistoria(idEstudiante, id);

                // 3. Recalcular promedio y actualizar
                var promedioResult = _evaluacionService.CalcularPromedioEstudiante(idEstudiante);
                if (promedioResult.Success)
                {
                    _estudianteService.ActualizarPromedio(idEstudiante, promedioResult.Data);
                }

                return Result<bool>.Ok(true, "Evaluación eliminada correctamente.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al eliminar evaluación: {ex.Message}", false);
            }
        }

        // ── Cancelación de Materia ────────────────────────────────

        /// <summary>
        /// Cancela una materia para un estudiante y publica EventoCancelarMateria.
        /// </summary>
        public Result<bool> CancelarMateria(CancelacionMateria cancelacion)
        {
            try
            {
                if (cancelacion == null)
                    return Result<bool>.Fail("Cancelación no puede ser nula.", false);

                if (string.IsNullOrWhiteSpace(cancelacion.Estudiante?.Id))
                    return Result<bool>.Fail("ID de estudiante es requerido.", false);

                cancelacion.CambiarEstado("Cancelada");

                var evento = new EventoCancelarMateria(
                    Guid.NewGuid().ToString(),
                    DateTime.Now,
                    cancelacion.Estudiante.Id,
                    $"Materia cancelada: {cancelacion.Asignatura.Nombre} ({cancelacion.Asignatura.Codigo})",
                    cancelacion.Asignatura.Codigo,
                    cancelacion.Motivo,
                    cancelacion.FechaCancelacion
                );

                var publicarResult = _gestorEventos.PublicarEvento(evento);
                if (!publicarResult.Success)
                    return Result<bool>.Fail($"Error publicando evento: {publicarResult.Message}", false);

                return Result<bool>.Ok(true, "Materia cancelada exitosamente.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al cancelar materia: {ex.Message}", false);
            }
        }

        // ── Graduación (con validaciones completas) ───────────────

        /// <summary>
        /// Gradúa a un estudiante SOLO si cumple todos los requisitos académicos.
        /// CORRECCIÓN: antes graduaba sin verificar ninguna condición.
        ///
        /// PREGRADO: promedio >= 3.0, requisito de grado configurado, nota práctica >= 3.0.
        /// POSGRADO: tesis presentada, nota cualitativa = "Aprobó" o "Laureada", promedio >= 3.0.
        /// </summary>
        public Result<bool> GraduarEstudiante(string idEstudiante)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(idEstudiante))
                    return Result<bool>.Fail("ID de estudiante no puede estar vacío.", false);

                var estudianteResult = _estudianteService.ObtenerPersona(idEstudiante);
                if (!estudianteResult.Success)
                    return Result<bool>.Fail(estudianteResult.Message, false);

                var estudiante = estudianteResult.Data as Estudiante;
                if (estudiante == null)
                    return Result<bool>.Fail("La persona no es un estudiante.", false);

                // ── Validación común ──────────────────────────────
                if (estudiante.Estado == "Crítico")
                    return Result<bool>.Fail(
                        "El estudiante está en estado crítico y no puede graduarse.", false);

                if (estudiante.Promedio < ValidadorAcademico.NOTA_MINIMA_APROBATORIA)
                    return Result<bool>.Fail(
                        $"El promedio del estudiante ({estudiante.Promedio:F2}) es inferior al mínimo requerido (3.0). No puede graduarse.", false);

                // ── Validaciones específicas por tipo ─────────────
                if (estudiante is EstudiantePregrado pregrado)
                {
                    var resultadoValidacion = ValidarRequisitosGradoPregrado(pregrado);
                    if (!resultadoValidacion.Success)
                        return resultadoValidacion;
                }
                else if (estudiante is EstudiantePosgrado posgrado)
                {
                    var resultadoValidacion = ValidarRequisitosGradoPosgrado(posgrado);
                    if (!resultadoValidacion.Success)
                        return resultadoValidacion;
                }

                // ── Graduar ───────────────────────────────────────
                estudiante.CambiarEstado("Graduado");

                var evento = new EventoGradoAprobado(
                    Guid.NewGuid().ToString(),
                    DateTime.Now,
                    idEstudiante,
                    $"Estudiante {estudiante.Nombre} ha cumplido todos los requisitos y fue graduado.",
                    idEstudiante,
                    "Grado",
                    DateTime.Now,
                    estudiante.Promedio
                );

                _gestorEventos.PublicarEvento(evento);
                return Result<bool>.Ok(true, $"✔ {estudiante.Nombre} ha sido graduado exitosamente.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al graduar estudiante: {ex.Message}", false);
            }
        }

        private Result<bool> ValidarRequisitosGradoPregrado(EstudiantePregrado pregrado)
        {
            if (string.IsNullOrWhiteSpace(pregrado.TipoPractica))
                return Result<bool>.Fail(
                    "El estudiante de pregrado no tiene un requisito de grado configurado " +
                    "(Práctica, Pasantía Investigativa o Plan de Negocios).", false);

            if (pregrado.NotaPractica < ValidadorAcademico.NOTA_MINIMA_APROBATORIA)
                return Result<bool>.Fail(
                    $"La nota del requisito de grado ({pregrado.NotaPractica:F1}) no es aprobatoria (mínimo 3.0).", false);

            return Result<bool>.Ok(true);
        }

        private Result<bool> ValidarRequisitosGradoPosgrado(EstudiantePosgrado posgrado)
        {
            if (!posgrado.TesisPresentada || string.IsNullOrWhiteSpace(posgrado.TituloTesis))
                return Result<bool>.Fail(
                    "El estudiante de posgrado no tiene una tesis registrada. La tesis es obligatoria para graduarse.", false);

            var notasAprobatorias = EstudiantePosgrado.NotasCualitativasValidas
                .Where(n => n != "No aprobó")
                .ToList();

            if (!notasAprobatorias.Contains(posgrado.NotaCualitativa))
                return Result<bool>.Fail(
                    $"La tesis tiene nota '{posgrado.NotaCualitativa}'. Solo se puede graduar con nota 'Aprobó' o 'Laureada'.", false);

            return Result<bool>.Ok(true);
        }

        // ── Análisis con Eventos Observer ─────────────────────────

        /// <summary>
        /// Recorre todas las evaluaciones y genera:
        ///   EventoAlertaConOportunidad  → cuando el estudiante aún puede aprobar matemáticamente.
        ///   EventoAlertaSinOportunidad  → cuando el estudiante ya no puede aprobar.
        ///
        /// Criterio: nota actual < 3.0.
        ///   Con oportunidad: nota actual >= 1.5 (puede recuperarse si la próxima evaluación es alta).
        ///   Sin oportunidad: nota actual < 1.5 (incluso con 5.0 el promedio no llegaría a 3.0).
        ///
        /// Ambos eventos son publicados al GestorEventosAcademicos (Observer).
        /// PermanenciaService y DirectivoService son notificados automáticamente.
        /// </summary>
        public Result<List<EventoAcademicoBase>> AnalizarEvaluacionesConEventos()
        {
            try
            {
                var todasResult = _evaluacionService.ObtenerTodas();
                if (!todasResult.Success)
                    return Result<List<EventoAcademicoBase>>.Fail(todasResult.Message, new());

                var eventosGenerados = new List<EventoAcademicoBase>();

                // Agrupar evaluaciones por estudiante para tener el promedio actualizado
                var porEstudiante = todasResult.Data
                    .Where(e => e.NotaFinal < ValidadorAcademico.NOTA_MINIMA_APROBATORIA)
                    .GroupBy(e => e.Estudiante.Id)
                    .ToList();

                foreach (var grupo in porEstudiante)
                {
                    var estudianteResult = _estudianteService.ObtenerPersona(grupo.Key);
                    if (!estudianteResult.Success || estudianteResult.Data is not Estudiante est)
                        continue;

                    foreach (var evaluacion in grupo)
                    {
                        EventoAcademicoBase evento;

                        // Umbral: si nota actual >= 1.5, aún puede recuperarse en futuras evaluaciones
                        bool puedeRecuperar = evaluacion.NotaFinal >= 1.5;

                        if (puedeRecuperar)
                        {
                            // Nota mínima necesaria para que el promedio sea 3.0
                            double notaMinimaNecesaria = Math.Min(
                                (ValidadorAcademico.NOTA_MINIMA_APROBATORIA * 2) - evaluacion.NotaFinal,
                                ValidadorAcademico.NOTA_ESCALA_MAXIMA);

                            evento = new EventoAlertaConOportunidad(
                                Guid.NewGuid().ToString(),
                                DateTime.Now,
                                est.Id,
                                $"Rendimiento bajo con oportunidad de recuperación en {evaluacion.Asignatura.Nombre}",
                                evaluacion.Asignatura.Nombre,
                                evaluacion.NotaFinal,
                                notaMinimaNecesaria,
                                est.Promedio
                            );
                        }
                        else
                        {
                            // Nota máxima posible (promedio de nota actual + 5.0) aún < 3.0
                            double notaMaximaPosible = (evaluacion.NotaFinal + ValidadorAcademico.NOTA_ESCALA_MAXIMA) / 2;

                            evento = new EventoAlertaSinOportunidad(
                                Guid.NewGuid().ToString(),
                                DateTime.Now,
                                est.Id,
                                $"Estudiante sin posibilidad de aprobar {evaluacion.Asignatura.Nombre}",
                                evaluacion.Asignatura.Nombre,
                                evaluacion.NotaFinal,
                                notaMaximaPosible,
                                est.Promedio
                            );
                        }

                        _gestorEventos.PublicarEvento(evento);
                        eventosGenerados.Add(evento);
                    }
                }

                return Result<List<EventoAcademicoBase>>.Ok(eventosGenerados,
                    $"{eventosGenerados.Count} evento(s) de alerta generados.");
            }
            catch (Exception ex)
            {
                return Result<List<EventoAcademicoBase>>.Fail(
                    $"Error en análisis de evaluaciones: {ex.Message}", new());
            }
        }

        // ── Estudiantes en Riesgo ─────────────────────────────────

        /// <summary>
        /// Retorna la lista de estudiantes con problemas de permanencia detectados por el analizador.
        /// </summary>
        public Result<List<Estudiante>> ObtenerEstudiantesEnRiesgo()
        {
            try
            {
                var todasPersonas = _estudianteService.ObtenerTodas();
                if (!todasPersonas.Success)
                    return Result<List<Estudiante>>.Fail(todasPersonas.Message, new List<Estudiante>());

                var estudiantes = todasPersonas.Data.OfType<Estudiante>().ToList();
                var enRiesgo    = new List<Estudiante>();

                foreach (var est in estudiantes)
                {
                    var tieneProblemas = _analizador.DetectarProblemasPermanencia(est);
                    if (tieneProblemas.Success && tieneProblemas.Data)
                        enRiesgo.Add(est);
                }

                return Result<List<Estudiante>>.Ok(enRiesgo);
            }
            catch (Exception ex)
            {
                return Result<List<Estudiante>>.Fail(
                    $"Error al obtener estudiantes en riesgo: {ex.Message}", new List<Estudiante>());
            }
        }
    }
}
