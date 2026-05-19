namespace SistemaAcademico.Domain.Models
{
    public class EventoGradoAprobado : EventoAcademicoBase
    {
        public string IdEstudiante { get; private set; }
        public string NivelGrado { get; private set; }
        public DateTime FechaGrado { get; private set; }
        public double PromedioFinal { get; private set; }

        public EventoGradoAprobado(string id, DateTime fechaOcurrencia, string idPersona,
            string descripcion, string IdEstudiante, string nivelGrado, DateTime fechaGrado, double promedioFinal)
            : base(id, fechaOcurrencia, idPersona, descripcion)
        {
            IdEstudiante = IdEstudiante;
            NivelGrado = nivelGrado;
            FechaGrado = fechaGrado;
            PromedioFinal = promedioFinal;
        }

        public override string ObtenerTipo()
        {
            return "Grado Aprobado";
        }

        public override string GenerarResumen()
        {
            return $"Estudiante {IdEstudiante} se ha graduado en {NivelGrado} - Promedio final: {PromedioFinal:F2}";
        }

        public override bool EsCritico()
        {
            return false;
        }
    }
}




