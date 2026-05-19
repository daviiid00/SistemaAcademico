namespace SistemaAcademico.Domain.Models
{
    public class Directivo : Persona
    {
        public string Cargo { get; private set; }
        public string Facultad { get; private set; }
        public DateTime FechaAsignacion { get; private set; }
        public bool Activo { get; private set; }

        public Directivo(string id, string nombre, DateTime fechaNacimiento, string email,
            string telefono, string cargo, string facultad, DateTime fechaAsignacion, bool activo)
            : base(id, nombre, fechaNacimiento, email, telefono)
        {
            Cargo = cargo;
            Facultad = facultad;
            FechaAsignacion = fechaAsignacion;
            Activo = activo;
        }

        public void ActivarDirectivo()
        {
            Activo = true;
        }

        public void DesactivarDirectivo()
        {
            Activo = false;
        }

        public override string ObtenerRol()
        {
            return "Directivo";
        }
    }
}
