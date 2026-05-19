namespace SistemaAcademico.Domain.Models
{
    public class HistoriaAcademica
    {
        public string IdEstudiante { get; private set; }
        public List<Evaluacion> Evaluaciones { get; private set; }
        public List<string> AsignaturasAprobadas { get; private set; }
        public List<string> AsignaturasReprobadas { get; private set; }
        public double PromedioGeneral { get; private set; }
        public int Año { get; private set; }
        public string Periodo { get; private set; }

        public HistoriaAcademica(string idEstudiante, int año, string periodo)
        {
            IdEstudiante = idEstudiante;
            Evaluaciones = new List<Evaluacion>();
            AsignaturasAprobadas = new List<string>();
            AsignaturasReprobadas = new List<string>();
            PromedioGeneral = 0.0;
            Año = año;
            Periodo = periodo;
        }

        public void AdicionarEvaluacion(Evaluacion evaluacion)
        {
            Evaluaciones.Add(evaluacion);
        }

        public void ActualizarPromedio(double nuevoPromedio)
        {
            PromedioGeneral = nuevoPromedio;
        }

        public void MarcarAsignaturaAprobada(string codigoAsignatura)
        {
            AsignaturasAprobadas.Add(codigoAsignatura);
        }

        public void MarcarAsignaturaReprobada(string codigoAsignatura)
        {
            AsignaturasReprobadas.Add(codigoAsignatura);
        }
    }
}




