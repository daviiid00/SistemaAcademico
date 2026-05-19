namespace SistemaAcademico.Domain.Models
{
    public abstract class Persona
    {
        public string Id { get; private set; }
        public string Nombre { get; private set; }
        public DateTime FechaNacimiento { get; private set; }
        public string Email { get; private set; }
        public string Telefono { get; private set; }

        protected Persona(string id, string nombre, DateTime fechaNacimiento, string email, string telefono)
        {
            Id = id;
            Nombre = nombre;
            FechaNacimiento = fechaNacimiento;
            Email = email;
            Telefono = telefono;
        }

        public int ObtenerEdad()
        {
            return DateTime.Now.Year - FechaNacimiento.Year;
        }

        public abstract string ObtenerRol();
    }
}
