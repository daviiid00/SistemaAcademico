namespace SistemaAcademico.Domain.Models
{
    public class EventoEvaluacionRegistrada : EventoAcademicoBase
    {
        public string IdEvaluacion { get; private set; }
        public double NotaFinal { get; private set; }
        public string Asignatura { get; private set; }

        public EventoEvaluacionRegistrada(string id, DateTime fechaOcurrencia, string idPersona,
            string descripcion, string idEvaluacion, double notaFinal, string asignatura)
            : base(id, fechaOcurrencia, idPersona, descripcion)
        {
            IdEvaluacion = idEvaluacion;
            NotaFinal = notaFinal;
            Asignatura = asignatura;
        }

        public override string ObtenerTipo()
        {
            return "Evaluacion Registrada";
        }

        public override string GenerarResumen()
        {
            return $"Evaluación registrada: {Asignatura} - Nota: {NotaFinal:F2} - Estudiante: {IdPersona}";
        }

        public override bool EsCritico()
        {
            return NotaFinal < 3.0;
        }
    }
}
