using SistemaAcademico.Domain.Models;

namespace Front.Models
{
    // ── ViewModel para Estudiante ─────────────────────────────
    public class EstudianteViewModel
    {
        public string   Id          { get; set; } = "";
        public string   Nombre      { get; set; } = "";
        public string   Email       { get; set; } = "";
        public string   Matricula   { get; set; } = "";
        public string   Estado      { get; set; } = "";
        public string   Acudiente   { get; set; } = "";
        public string   Tipo        { get; set; } = "";
        public double   Promedio    { get; set; }
        public DateTime FechaNac    { get; set; }
        public string   ProgramaOEspecialidad { get; set; } = "";

        // Pregrado
        public int    SemestreActual   { get; set; }
        public int    CreditosCursados { get; set; }

        // Bachillerato (Pregrado)
        public string ColegioEgresado         { get; set; } = "";
        public int    AñoGraduacionBachillerato { get; set; }
        public string TituloDiploma           { get; set; } = "";

        // Posgrado
        public string Maestria        { get; set; } = "";
        public bool   TesisPresentada { get; set; }
        public string TituloTesis     { get; set; } = "";
        public string DirectorTesis   { get; set; } = "";
        public string NotaCualitativa { get; set; } = "";

        // Estudios Previos (Posgrado)
        public List<EstudioPrevioViewModel> EstudiosPrevios { get; set; } = new();

        public string BadgeClase => Tipo == "Pregrado" ? "badge-blue" : "badge-green";
        public int    Edad       => DateTime.Now.Year - FechaNac.Year;

        public static EstudianteViewModel FromDomain(Persona persona)
        {
            if (persona is not Estudiante e) return new EstudianteViewModel();

            var vm = new EstudianteViewModel
            {
                Id        = e.Id,
                Nombre    = e.Nombre,
                Email     = e.Email,
                Matricula = e.Matricula,
                Estado    = e.Estado,
                Acudiente = e.NombreAcudiente,
                Promedio  = e.Promedio,
                FechaNac  = e.FechaNacimiento
            };

            if (e is EstudiantePregrado pre)
            {
                vm.Tipo                 = "Pregrado";
                vm.SemestreActual       = pre.SemestreActual;
                vm.CreditosCursados     = pre.CreditosCursados;
                vm.ProgramaOEspecialidad = pre.ProgramaAcademico;

                if (pre.DatosBachillerato != null)
                {
                    vm.ColegioEgresado           = pre.DatosBachillerato.Colegio;
                    vm.AñoGraduacionBachillerato = pre.DatosBachillerato.AñoGraduacion;
                    vm.TituloDiploma             = pre.DatosBachillerato.TituloDiploma;
                }
            }
            else if (e is EstudiantePosgrado pos)
            {
                vm.Tipo                  = "Posgrado";
                vm.Maestria              = pos.Maestria;
                vm.TesisPresentada       = pos.TesisPresentada;
                vm.ProgramaOEspecialidad = pos.Maestria;
                vm.TituloTesis           = pos.TituloTesis;
                vm.DirectorTesis         = pos.DirectorTesis;
                vm.NotaCualitativa       = pos.NotaCualitativa;
                vm.EstudiosPrevios       = pos.EstudiosPrevios
                    .Select(ep => new EstudioPrevioViewModel
                    {
                        Institucion    = ep.Institucion,
                        TituloObtenido = ep.TituloObtenido,
                        AñoGraduacion  = ep.AñoGraduacion
                    }).ToList();
            }

            return vm;
        }
    }

    // ── ViewModel para Estudio Previo (Posgrado) ──────────────
    public class EstudioPrevioViewModel
    {
        public string Institucion    { get; set; } = "";
        public string TituloObtenido { get; set; } = "";
        public int    AñoGraduacion  { get; set; }
    }

    // ── ViewModel para Historia Académica ─────────────────────
    public class HistoriaAcademicaViewModel
    {
        public string                   IdEstudiante        { get; set; } = "";
        public List<EvaluacionViewModel> Evaluaciones       { get; set; } = new();
        public List<string>             AsignaturasAprobadas { get; set; } = new();
        public List<string>             AsignaturasReprobadas { get; set; } = new();
        public double                   PromedioGeneral     { get; set; }
        public int                      TotalCancelaciones  { get; set; }
    }

    // ── ViewModel para Docente ─────────────────────────────────
    public class DocenteViewModel
    {
        public string       Id               { get; set; } = "";
        public string       Nombre           { get; set; } = "";
        public string       Email            { get; set; } = "";
        public string       Departamento     { get; set; } = "";
        public string       Area             { get; set; } = "";
        public List<string> Titulos          { get; set; } = new();
        public bool         Activo           { get; set; }
        public double       Salario          { get; set; }
        public string       NumeroEmpleado   { get; set; } = "";
        public DateTime     FechaNacimiento  { get; set; }
        public List<string> AsignaturasAsignadas { get; set; } = new();

        public string BadgeEstado => Activo ? "badge-green" : "badge-gray";
        public int    Edad        => DateTime.Now.Year - FechaNacimiento.Year;

        public static DocenteViewModel FromDomain(Persona persona)
        {
            if (persona is not Docente d) return new DocenteViewModel();
            return new DocenteViewModel
            {
                Id              = d.Id,
                Nombre          = d.Nombre,
                Email           = d.Email,
                Departamento    = d.Departamento,
                Area            = d.Area,
                Titulos         = d.Titulos ?? new(),
                Activo          = d.Activo,
                Salario         = d.Salario,
                NumeroEmpleado  = d.NumeroEmpleado,
                FechaNacimiento = d.FechaNacimiento
            };
        }
    }

    // ── ViewModel para Asignatura ──────────────────────────────
    public class AsignaturaViewModel
    {
        public string       Codigo          { get; set; } = "";
        public string       Nombre          { get; set; } = "";
        public string       Grado           { get; set; } = "";
        public int          Creditos        { get; set; }
        public string       Area            { get; set; } = "";
        public List<string> NombresDocentes { get; set; } = new();

        public static AsignaturaViewModel FromDomain(Asignatura a) => new()
        {
            Codigo          = a.Codigo,
            Nombre          = a.Nombre,
            Grado           = a.Grado,
            Creditos        = a.Creditos,
            Area            = a.Departamento,
            NombresDocentes = a.DocentesAsignados?.Select(d => d.Nombre).ToList() ?? new()
        };
    }

    // ── ViewModel para Evaluación ──────────────────────────────
    public class EvaluacionViewModel
    {
        public string CodigoEval       { get; set; } = "";
        public string NombreEstudiante { get; set; } = "";
        public string IdEstudiante     { get; set; } = "";
        public string NombreAsignatura { get; set; } = "";
        public string CodigoAsignatura { get; set; } = "";
        public double NotaFinal        { get; set; }
        public string Periodo          { get; set; } = "";

        public string BadgeNota => NotaFinal >= 3.0 ? "badge-green" : "badge-red";
        public string TextoNota => NotaFinal.ToString("0.0");

        public static EvaluacionViewModel FromDomain(Evaluacion ev) => new()
        {
            CodigoEval       = ev.Id,
            NombreEstudiante = ev.Estudiante?.Nombre ?? "—",
            IdEstudiante     = ev.Estudiante?.Id     ?? "",
            NombreAsignatura = ev.Asignatura?.Nombre ?? "—",
            CodigoAsignatura = ev.Asignatura?.Codigo ?? "—",
            NotaFinal        = ev.NotaFinal,
            Periodo          = ev.Descripcion ?? ev.Tipo
        };
    }

    // ── ViewModel para Reportes ────────────────────────────────
    public class ReporteAcademicoViewModel
    {
        public int    EstudiantesAprobados        { get; set; }
        public int    EstudiantesPerdidos         { get; set; }
        public string AsignaturaMejorPromedio     { get; set; } = "N/A";
        public string AsignaturaPeorPromedio      { get; set; } = "N/A";
        public double PromedioGeneralUniversidad  { get; set; }
        // Nuevos campos
        public int    EvaluacionesAprobadas       { get; set; }
        public int    EvaluacionesReprobadas      { get; set; }
        public string AsignaturaMasPerdida        { get; set; } = "N/A";
        public int    EstudiantesEnRiesgoCount    { get; set; }
    }

    // ── Stats del Dashboard ────────────────────────────────────
    public class DashboardStats
    {
        public int    TotalEstudiantes  { get; set; }
        public int    TotalDocentes     { get; set; }
        public int    TotalAsignaturas  { get; set; }
        public int    TotalEvaluaciones { get; set; }
        public double PromedioGeneral   { get; set; }
        public int    EvaluacionesHoy   { get; set; }
        public int    AlertasActivas    { get; set; }
    }
}
