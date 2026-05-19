namespace SistemaAcademico.Domain.Models
{
    public class Asignatura
    {
        public string Codigo { get; private set; }
        public string Nombre { get; private set; }
        public int Creditos { get; private set; }
        public string Departamento { get; private set; }
        public double HorasTeoria { get; private set; }
        public double HorasPractica { get; private set; }
        public string Grado { get; private set; }
        public List<Docente> DocentesAsignados { get; private set; }

        public Asignatura(string codigo, string nombre, int creditos, string departamento,
            double horasTeoria, double horasPractica, string grado, List<Docente> docentesAsignados)
        {
            Codigo = codigo;
            Nombre = nombre;
            Creditos = creditos;
            Departamento = departamento;
            HorasTeoria = horasTeoria;
            HorasPractica = horasPractica;
            Grado = grado;
            DocentesAsignados = docentesAsignados ?? new List<Docente>();
        }
    }
}
