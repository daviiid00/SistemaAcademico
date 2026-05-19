namespace SistemaAcademico.Domain.Models
{
    public class CancelacionMateria
    {
        public string Id { get; private set; }
        public Estudiante Estudiante { get; private set; }
        public Asignatura Asignatura { get; private set; }
        public DateTime FechaCancelacion { get; private set; }
        public string Motivo { get; private set; }
        public string Estado { get; private set; }

        public CancelacionMateria(string id, Estudiante estudiante, Asignatura asignatura,
            DateTime fechaCancelacion, string motivo, string estado)
        {
            Id = id;
            Estudiante = estudiante;
            Asignatura = asignatura;
            FechaCancelacion = fechaCancelacion;
            Motivo = motivo;
            Estado = estado;
        }

        public void CambiarEstado(string nuevoEstado)
        {
            Estado = nuevoEstado;
        }
    }
}


