namespace SistemaAcademico.Domain.Models
{
    /// <summary>
    /// Evento que se genera cuando un estudiante tiene rendimiento bajo
    /// pero TODAVÍA puede aprobar la asignatura matemáticamente.
    /// 
    /// Publisher  : GestorEventosAcademicos
    /// Subscribers: PermanenciaService, DirectivoService
    /// 
    /// Se dispara en: UniversidadService.AnalizarEvaluacionesConEventos()
    /// </summary>
    public class EventoAlertaConOportunidad : EventoAcademicoBase
    {
        /// <summary>Nombre de la asignatura en riesgo.</summary>
        public string NombreAsignatura { get; private set; }

        /// <summary>Nota actual del estudiante en la asignatura.</summary>
        public double NotaActual { get; private set; }

        /// <summary>Nota mínima que necesita para poder aprobar.</summary>
        public double NotaMinimaNecesaria { get; private set; }

        /// <summary>Promedio general actual del estudiante.</summary>
        public double PromedioEstudiante { get; private set; }

        public EventoAlertaConOportunidad(
            string   id,
            DateTime fechaOcurrencia,
            string   idPersona,
            string   descripcion,
            string   nombreAsignatura,
            double   notaActual,
            double   notaMinimaNecesaria,
            double   promedioEstudiante)
            : base(id, fechaOcurrencia, idPersona, descripcion)
        {
            NombreAsignatura    = nombreAsignatura;
            NotaActual          = notaActual;
            NotaMinimaNecesaria = notaMinimaNecesaria;
            PromedioEstudiante  = promedioEstudiante;
        }

        public override string ObtenerTipo()
            => "Alerta Con Oportunidad";

        public override string GenerarResumen()
            => $"[CON OPORTUNIDAD] Estudiante {IdPersona} — {NombreAsignatura}: " +
               $"nota actual {NotaActual:F1}, necesita ≥ {NotaMinimaNecesaria:F1} para aprobar. " +
               $"Promedio general: {PromedioEstudiante:F2}.";

        /// <summary>
        /// Este evento es de alerta preventiva, no es crítico todavía.
        /// Se vuelve crítico solo si el promedio general está por debajo de 2.5.
        /// </summary>
        public override bool EsCritico()
            => PromedioEstudiante < 2.5;
    }
}
