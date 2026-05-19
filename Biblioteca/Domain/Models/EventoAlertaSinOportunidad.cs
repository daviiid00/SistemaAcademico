namespace SistemaAcademico.Domain.Models
{
    /// <summary>
    /// Evento que se genera cuando un estudiante YA NO puede aprobar
    /// la asignatura matemáticamente, sin importar notas futuras.
    /// 
    /// Publisher  : GestorEventosAcademicos
    /// Subscribers: PermanenciaService, DirectivoService
    /// 
    /// Se dispara en: UniversidadService.AnalizarEvaluacionesConEventos()
    /// Este evento SIEMPRE es crítico (EsCritico() = true).
    /// </summary>
    public class EventoAlertaSinOportunidad : EventoAcademicoBase
    {
        /// <summary>Nombre de la asignatura definitivamente perdida.</summary>
        public string NombreAsignatura { get; private set; }

        /// <summary>Nota actual del estudiante en la asignatura.</summary>
        public double NotaActual { get; private set; }

        /// <summary>Nota máxima posible que podría alcanzar (ya es insuficiente).</summary>
        public double NotaMaximaPosible { get; private set; }

        /// <summary>Promedio general actual del estudiante.</summary>
        public double PromedioEstudiante { get; private set; }

        public EventoAlertaSinOportunidad(
            string   id,
            DateTime fechaOcurrencia,
            string   idPersona,
            string   descripcion,
            string   nombreAsignatura,
            double   notaActual,
            double   notaMaximaPosible,
            double   promedioEstudiante)
            : base(id, fechaOcurrencia, idPersona, descripcion)
        {
            NombreAsignatura  = nombreAsignatura;
            NotaActual        = notaActual;
            NotaMaximaPosible = notaMaximaPosible;
            PromedioEstudiante = promedioEstudiante;
        }

        public override string ObtenerTipo()
            => "Alerta Sin Oportunidad";

        public override string GenerarResumen()
            => $"[SIN OPORTUNIDAD] Estudiante {IdPersona} — {NombreAsignatura}: " +
               $"nota actual {NotaActual:F1}, máximo posible {NotaMaximaPosible:F1} (< 3.0). " +
               $"Promedio general: {PromedioEstudiante:F2}. Requiere intervención inmediata.";

        /// <summary>
        /// Este evento SIEMPRE es crítico — el estudiante ya perdió la asignatura.
        /// </summary>
        public override bool EsCritico() => true;
    }
}
