namespace SistemaAcademico.Domain.Models
{
    public class Tesis
    {
        public string Id { get; private set; }
        public string IdEstudiante { get; private set; }
        public string Titulo { get; private set; }
        public string Asesor { get; private set; }
        public DateTime FechaRegistro { get; private set; }
        public DateTime? FechaDefensa { get; private set; }
        public double Calificacion { get; private set; }
        public string Estado { get; private set; }

        public Tesis(string id, string idEstudiante, string titulo, string asesor,
            DateTime fechaRegistro, DateTime? fechaDefensa, double calificacion, string estado)
        {
            Id = id;
            IdEstudiante = idEstudiante;
            Titulo = titulo;
            Asesor = asesor;
            FechaRegistro = fechaRegistro;
            FechaDefensa = fechaDefensa;
            Calificacion = calificacion;
            Estado = estado;
        }

        public void RegistrarDefensa(DateTime fecha)
        {
            FechaDefensa = fecha;
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



