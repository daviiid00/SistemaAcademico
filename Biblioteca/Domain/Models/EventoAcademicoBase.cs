namespace SistemaAcademico.Domain.Models
{
    public abstract class EventoAcademicoBase
    {
        public string Id { get; private set; }
        public DateTime FechaOcurrencia { get; private set; }
        public string IdPersona { get; private set; }
        public string Descripcion { get; private set; }

        protected EventoAcademicoBase(string id, DateTime fechaOcurrencia, string idPersona, string descripcion)
        {
            Id = id;
            FechaOcurrencia = fechaOcurrencia;
            IdPersona = idPersona;
            Descripcion = descripcion;
        }

        public abstract string ObtenerTipo();
        public abstract string GenerarResumen();
        public abstract bool EsCritico();
    }
}
