namespace SistemaAcademico.Domain.Models
{
    public abstract class Estudiante : Persona
    {
        public string Matricula { get; private set; }
        public DateTime FechaIngreso { get; private set; }
        public double Promedio { get; private set; }
        public string Estado { get; private set; }
        public string NombreAcudiente { get; private set; }

        protected Estudiante(string id, string nombre, DateTime fechaNacimiento, string email, 
            string telefono, string matricula, DateTime fechaIngreso, double promedio, string estado, string nombreAcudiente)
            : base(id, nombre, fechaNacimiento, email, telefono)
        {
            Matricula = matricula;
            FechaIngreso = fechaIngreso;
            Promedio = promedio;
            Estado = estado;
            NombreAcudiente = nombreAcudiente;
        }

        public virtual void ActualizarPromedio(double nuevoPromedio)
        {
            Promedio = nuevoPromedio;
        }

        public virtual void CambiarEstado(string nuevoEstado)
        {
            Estado = nuevoEstado;
        }

        public override string ObtenerRol()
        {
            return "Estudiante";
        }

        public abstract string ObtenerRequisitosDeGrado();
    }
}
