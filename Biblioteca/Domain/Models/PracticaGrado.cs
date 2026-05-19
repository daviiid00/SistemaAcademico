namespace SistemaAcademico.Domain.Models
{
    public class PracticaGrado
    {
        public string Id { get; private set; }
        public string IdEstudiante { get; private set; }
        public string Empresa { get; private set; }
        public DateTime FechaInicio { get; private set; }
        public DateTime FechaFin { get; private set; }
        public string Supervisor { get; private set; }
        public double Calificacion { get; private set; }
        public string Estado { get; private set; }

        public PracticaGrado(string id, string idEstudiante, string empresa, DateTime fechaInicio,
            DateTime fechaFin, string supervisor, double calificacion, string estado)
        {
            Id = id;
            IdEstudiante = idEstudiante;
            Empresa = empresa;
            FechaInicio = fechaInicio;
            FechaFin = fechaFin;
            Supervisor = supervisor;
            Calificacion = calificacion;
            Estado = estado;
        }

        public void ActualizarCalificacion(double nuevaCalificacion)
        {
            Calificacion = nuevaCalificacion;
        }

        public void CambiarEstado(string nuevoEstado)
        {
            Estado = nuevoEstado;
        }
    }
}



