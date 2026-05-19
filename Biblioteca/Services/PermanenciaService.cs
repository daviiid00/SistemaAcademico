namespace SistemaAcademico.Services
{
    using Domain.Models;
    using Interfaces;

    /// <summary>
    /// Observer que gestiona la permanencia académica de los estudiantes.
    /// Se suscribe a GestorEventosAcademicos y reacciona a eventos de riesgo.
    ///
    /// Suscrito a:
    ///   EventoEvaluacionRegistrada     → registra alerta si nota < 3.0
    ///   EventoAlertaConOportunidad     → registra alerta preventiva
    ///   EventoAlertaSinOportunidad     → registra caso crítico
    ///   EventoCancelarMateria          → registra cancelación
    /// </summary>
    public class PermanenciaService : IObservadorAcademico
    {
        private readonly EstudianteService _estudianteService;

        // Contadores de seguimiento por estudiante
        private readonly Dictionary<string, int>              _semestresEnAlerta;
        private readonly Dictionary<string, List<string>>     _alertasConOportunidad;
        private readonly Dictionary<string, List<string>>     _alertasSinOportunidad;
        private readonly Dictionary<string, List<string>>     _cancelaciones;

        public PermanenciaService(EstudianteService estudianteService)
        {
            _estudianteService     = estudianteService
                ?? throw new ArgumentNullException(nameof(estudianteService));
            _semestresEnAlerta     = new Dictionary<string, int>();
            _alertasConOportunidad = new Dictionary<string, List<string>>();
            _alertasSinOportunidad = new Dictionary<string, List<string>>();
            _cancelaciones         = new Dictionary<string, List<string>>();
        }

        // ── IObservadorAcademico ──────────────────────────────────

        public Result<bool> Notificar(EventoAcademicoBase evento)
        {
            try
            {
                if (evento == null)
                    return Result<bool>.Fail("Evento no puede ser nulo.", false);

                return evento switch
                {
                    EventoEvaluacionRegistrada ev  => ProcesarEvaluacionRegistrada(ev),
                    EventoAlertaConOportunidad  ev  => ProcesarAlertaConOportunidad(ev),
                    EventoAlertaSinOportunidad  ev  => ProcesarAlertaSinOportunidad(ev),
                    EventoCancelarMateria       ev  => ProcesarCancelacion(ev),
                    _                               => Result<bool>.Ok(true, "Evento no relevante para Permanencia.")
                };
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error en PermanenciaService al notificar: {ex.Message}", false);
            }
        }

        // ── Procesadores privados ─────────────────────────────────

        private Result<bool> ProcesarEvaluacionRegistrada(EventoEvaluacionRegistrada evento)
        {
            if (evento.NotaFinal < 3.0)
            {
                _semestresEnAlerta.TryAdd(evento.IdPersona, 0);
                _semestresEnAlerta[evento.IdPersona]++;
            }
            return Result<bool>.Ok(true, "Evaluación procesada en Permanencia.");
        }

        private Result<bool> ProcesarAlertaConOportunidad(EventoAlertaConOportunidad evento)
        {
            _alertasConOportunidad.TryAdd(evento.IdPersona, new List<string>());
            _alertasConOportunidad[evento.IdPersona]
                .Add($"[{DateTime.Now:dd/MM/yyyy}] {evento.NombreAsignatura}: nota {evento.NotaActual:F1}, necesita ≥{evento.NotaMinimaNecesaria:F1}");

            return Result<bool>.Ok(true, $"Alerta con oportunidad registrada para {evento.IdPersona}.");
        }

        private Result<bool> ProcesarAlertaSinOportunidad(EventoAlertaSinOportunidad evento)
        {
            _alertasSinOportunidad.TryAdd(evento.IdPersona, new List<string>());
            _alertasSinOportunidad[evento.IdPersona]
                .Add($"[{DateTime.Now:dd/MM/yyyy}] {evento.NombreAsignatura}: nota {evento.NotaActual:F1} — PERDIDA DEFINITIVA");

            // Si ya perdió materias definitivamente, cambiar estado del estudiante
            var estudianteResult = _estudianteService.ObtenerPersona(evento.IdPersona);
            if (estudianteResult.Success && estudianteResult.Data is Estudiante est)
            {
                if (est.Estado != "Graduado")
                    est.CambiarEstado("En Riesgo");
            }

            return Result<bool>.Ok(true, $"Alerta sin oportunidad (crítica) registrada para {evento.IdPersona}.");
        }

        private Result<bool> ProcesarCancelacion(EventoCancelarMateria evento)
        {
            _cancelaciones.TryAdd(evento.IdPersona, new List<string>());
            _cancelaciones[evento.IdPersona]
                .Add($"[{evento.FechaCancelacion:dd/MM/yyyy}] {evento.CodigoAsignatura}: {evento.Motivo}");

            return Result<bool>.Ok(true, $"Cancelación registrada en Permanencia para {evento.IdPersona}.");
        }

        // ── Consultas públicas ────────────────────────────────────

        public int ObtenerSemestresEnAlerta(string idEstudiante)
        {
            return _semestresEnAlerta.TryGetValue(idEstudiante, out int val) ? val : 0;
        }

        public List<string> ObtenerAlertasConOportunidad(string idEstudiante)
        {
            return _alertasConOportunidad.TryGetValue(idEstudiante, out var lista)
                ? lista : new List<string>();
        }

        public List<string> ObtenerAlertasSinOportunidad(string idEstudiante)
        {
            return _alertasSinOportunidad.TryGetValue(idEstudiante, out var lista)
                ? lista : new List<string>();
        }

        public List<string> ObtenerCancelaciones(string idEstudiante)
        {
            return _cancelaciones.TryGetValue(idEstudiante, out var lista)
                ? lista : new List<string>();
        }
    }
}
