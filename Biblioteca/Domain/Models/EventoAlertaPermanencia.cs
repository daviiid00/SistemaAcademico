namespace SistemaAcademico.Domain.Models
{
    public class EventoAlertaPermanencia : EventoAcademicoBase
    {
        public double PromedioActual { get; private set; }
        public int SemestresEnAlerta { get; private set; }
        public string Mensaje { get; private set; }

        public EventoAlertaPermanencia(string id, DateTime fechaOcurrencia, string idPersona,
            string descripcion, double promedioActual, int semestresEnAlerta, string mensaje)
            : base(id, fechaOcurrencia, idPersona, descripcion)
        {
            PromedioActual = promedioActual;
            SemestresEnAlerta = semestresEnAlerta;
            Mensaje = mensaje;
        }

        public override string ObtenerTipo()
        {
            return "Alerta de Permanencia";
        }

        public override string GenerarResumen()
        {
            return $"Alerta de permanencia: Estudiante {IdPersona} - Promedio: {PromedioActual:F2} - Semestres en alerta: {SemestresEnAlerta}";
        }

        public override bool EsCritico()
        {
            return SemestresEnAlerta >= 2;
        }
    }
}
