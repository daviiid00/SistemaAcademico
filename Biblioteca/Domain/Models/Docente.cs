namespace SistemaAcademico.Domain.Models
{
    public class Docente : Persona
    {
        public string NumeroEmpleado { get; private set; }
        public string Departamento { get; private set; }
        public string Especialidad { get; private set; }
        public double Salario { get; private set; }
        public bool Activo { get; private set; }
        public List<string> Titulos { get; private set; }
        public string Area { get; private set; }

        public Docente(string id, string nombre, DateTime fechaNacimiento, string email,
            string telefono, string numeroEmpleado, string departamento, string especialidad,
            double salario, bool activo, List<string> titulos, string area)
            : base(id, nombre, fechaNacimiento, email, telefono)
        {
            NumeroEmpleado = numeroEmpleado;
            Departamento = departamento;
            Especialidad = especialidad;
            Salario = salario;
            Activo = activo;
            Titulos = titulos ?? new List<string>();
            Area = area;
        }

        public void ActivarDocente()
        {
            Activo = true;
        }

        public void DesactivarDocente()
        {
            Activo = false;
        }

        public override string ObtenerRol()
        {
            return "Docente";
        }
    }
}
