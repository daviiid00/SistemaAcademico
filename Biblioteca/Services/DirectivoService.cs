namespace SistemaAcademico.Services
{
    using Domain.Models;
    using Interfaces;

    /// <summary>
    /// Observer que representa al equipo directivo universitario.
    /// Recibe y registra TODOS los eventos académicos, diferenciando su criticidad.
    ///
    /// Suscrito a todos los eventos del GestorEventosAcademicos:
    ///   EventoEvaluacionRegistrada
    ///   EventoAlertaConOportunidad   → alerta preventiva
    ///   EventoAlertaSinOportunidad   → crítico — requiere acción directiva
    ///   EventoCancelarMateria        → crítico
    ///   EventoGradoAprobado          → informativo
    ///   EventoAlertaPermanencia      → alerta general
    /// </summary>
    public class DirectivoService : IObservadorAcademico
    {
        private readonly List<EventoAcademicoBase> _eventosRecibidos;

        // Contadores diferenciados por tipo de evento
        public int TotalAlertas            { get; private set; }
        public int TotalAlertasConOport    { get; private set; }
        public int TotalAlertasSinOport    { get; private set; }
        public int TotalCancelaciones      { get; private set; }
        public int TotalGraduaciones       { get; private set; }
        public int TotalEvaluaciones       { get; private set; }

        public DirectivoService()
        {
            _eventosRecibidos = new List<EventoAcademicoBase>();
        }

        // ── IObservadorAcademico ──────────────────────────────────

        public Result<bool> Notificar(EventoAcademicoBase evento)
        {
            try
            {
                if (evento == null)
                    return Result<bool>.Fail("Evento no puede ser nulo.", false);

                _eventosRecibidos.Add(evento);

                // Contabilizar por tipo
                switch (evento)
                {
                    case EventoAlertaConOportunidad:
                        TotalAlertasConOport++;
                        TotalAlertas++;
                        break;

                    case EventoAlertaSinOportunidad:
                        TotalAlertasSinOport++;
                        TotalAlertas++;
                        break;

                    case EventoCancelarMateria:
                        TotalCancelaciones++;
                        break;

                    case EventoGradoAprobado:
                        TotalGraduaciones++;
                        break;

                    case EventoEvaluacionRegistrada:
                        TotalEvaluaciones++;
                        break;

                    case EventoAlertaPermanencia:
                        TotalAlertas++;
                        break;
                }

                string nivel = evento.EsCritico() ? "🔴 CRÍTICO" : "🟡 Informativo";
                return Result<bool>.Ok(true,
                    $"[Directivo] {nivel} — {evento.ObtenerTipo()} registrado para estudiante {evento.IdPersona}.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error en DirectivoService al notificar: {ex.Message}", false);
            }
        }

        // ── Consultas públicas ────────────────────────────────────

        public Result<List<EventoAcademicoBase>> ObtenerEventosRecibidos()
        {
            try
            {
                return Result<List<EventoAcademicoBase>>.Ok(
                    _eventosRecibidos.OrderByDescending(e => e.FechaOcurrencia).ToList());
            }
            catch (Exception ex)
            {
                return Result<List<EventoAcademicoBase>>.Fail(
                    $"Error al obtener eventos: {ex.Message}", new List<EventoAcademicoBase>());
            }
        }

        public Result<List<EventoAcademicoBase>> ObtenerEventosCriticos()
        {
            try
            {
                var criticos = _eventosRecibidos
                    .Where(e => e.EsCritico())
                    .OrderByDescending(e => e.FechaOcurrencia)
                    .ToList();
                return Result<List<EventoAcademicoBase>>.Ok(criticos);
            }
            catch (Exception ex)
            {
                return Result<List<EventoAcademicoBase>>.Fail(
                    $"Error al obtener eventos críticos: {ex.Message}", new List<EventoAcademicoBase>());
            }
        }

        /// <summary>Retorna solo eventos del tipo especificado.</summary>
        public List<EventoAcademicoBase> ObtenerEventosPorTipo(string tipo)
        {
            return _eventosRecibidos
                .Where(e => e.ObtenerTipo() == tipo)
                .OrderByDescending(e => e.FechaOcurrencia)
                .ToList();
        }
    }
}
