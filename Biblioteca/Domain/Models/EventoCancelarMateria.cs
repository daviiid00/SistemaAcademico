namespace SistemaAcademico.Domain.Models
{
    public class EventoCancelarMateria : EventoAcademicoBase
    {
        public string CodigoAsignatura { get; private set; }
        public string Motivo { get; private set; }
        public DateTime FechaCancelacion { get; private set; }

        public EventoCancelarMateria(string id, DateTime fechaOcurrencia, string idPersona,
            string descripcion, string codigoAsignatura, string motivo, DateTime fechaCancelacion)
            : base(id, fechaOcurrencia, idPersona, descripcion)
        {
            CodigoAsignatura = codigoAsignatura;
            Motivo = motivo;
            FechaCancelacion = fechaCancelacion;
        }

        public override string ObtenerTipo()
        {
            return "Cancelacion de Materia";
        }

        public override string GenerarResumen()
        {
            return $"Materia cancelada: {CodigoAsignatura} - Estudiante: {IdPersona} - Motivo: {Motivo}";
        }

        public override bool EsCritico()
        {
            return true;
        }
    }
}




