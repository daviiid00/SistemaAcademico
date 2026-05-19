namespace SistemaAcademico.Domain.Models
{
    public class Evaluacion
    {
        public string Id { get; private set; }
        public Estudiante Estudiante { get; private set; }
        public Asignatura Asignatura { get; private set; }
        public DateTime Fecha { get; private set; }
        public double NotaFinal { get; private set; }
        public string Tipo { get; private set; }
        public string Descripcion { get; private set; }

        public Evaluacion(string id, Estudiante estudiante, Asignatura asignatura, DateTime fecha,
            double notaFinal, string tipo, string descripcion)
        {
            Id = id;
            Estudiante = estudiante;
            Asignatura = asignatura;
            Fecha = fecha;
            NotaFinal = notaFinal;
            Tipo = tipo;
            Descripcion = descripcion;
        }
    }
}
