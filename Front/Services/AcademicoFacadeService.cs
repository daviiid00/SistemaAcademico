using Front.Models;
using SistemaAcademico.Aspects;
using SistemaAcademico.Domain.Models;
using SistemaAcademico.Interfaces;
using SistemaAcademico.Services;

namespace Front.Services
{
    public class AcademicoFacadeService
    {
        private readonly EstudianteService       _estudianteService;
        private readonly DocenteService          _docenteService;
        private readonly AsignaturaService       _asignaturaService;
        private readonly EvaluacionService       _evaluacionService;
        private readonly UniversidadService      _universidadService;
        private readonly IGestorEventos          _gestorEventos;
        private readonly IAnalizadorEvaluaciones _analizador;

        // Lista local de evaluaciones para la vista (fuente de verdad del front)
        private readonly List<Evaluacion> _evalLocal = new();

        public AcademicoFacadeService(
            EstudianteService       estudianteService,
            DocenteService          docenteService,
            AsignaturaService       asignaturaService,
            EvaluacionService       evaluacionService,
            UniversidadService      universidadService,
            IGestorEventos          gestorEventos,
            IAnalizadorEvaluaciones analizador,
            PermanenciaService      permanenciaService,
            DirectivoService        directivoService)
        {
            _estudianteService  = estudianteService;
            _docenteService     = docenteService;
            _asignaturaService  = asignaturaService;
            _evaluacionService  = evaluacionService;
            _universidadService = universidadService;
            _gestorEventos      = gestorEventos;
            _analizador         = analizador;

            // Suscribir observers
            _gestorEventos.Suscribirse(permanenciaService);
            _gestorEventos.Suscribirse(directivoService);

            SembrarDatosDemostracion();
        }

        // ── Estudiantes ───────────────────────────────────────────

        public List<EstudianteViewModel> ObtenerEstudiantes()
        {
            var res = _estudianteService.ObtenerTodas();
            if (!res.Success || res.Data == null) return new();
            return res.Data.Select(EstudianteViewModel.FromDomain).ToList();
        }

        public EstudianteViewModel? ObtenerEstudiantePorId(string id)
        {
            var res = _estudianteService.ObtenerPersona(id);
            if (!res.Success || res.Data == null) return null;
            return EstudianteViewModel.FromDomain(res.Data);
        }

        /// <summary>
        /// Registra un estudiante con todos los campos completos.
        /// Valida nombre (solo letras) y aplica datos de bachillerato/estudios previos si se proporcionan.
        /// </summary>
        public Result<bool> RegistrarEstudiante(
            string   tipo,
            string   id,
            string   nombre,
            string   acudiente,
            string   programa,
            DateTime fechaNacimiento,
            string   matricula       = "",
            string   colegio         = "",
            int      añoBach         = 0,
            string   tituloDiploma   = "",
            string   instPrevia      = "",
            string   tituloObtenido  = "",
            int      añoGradPrevio   = 0)
        {
            try
            {
                // Validar nombre
                var nombreVal = ValidadorAcademico.ValidarNombreSoloLetras(nombre);
                if (!nombreVal.valido)
                    return Result<bool>.Fail(nombreVal.mensaje, false);

                string mat = string.IsNullOrWhiteSpace(matricula) ? id : matricula;

                // Verificar ID duplicado (la capa de PersonaService también lo hace, pero aquí damos mensaje claro)
                var existeId = _estudianteService.ObtenerPersona(id);
                if (existeId.Success)
                    return Result<bool>.Fail($"Ya existe un estudiante con el ID '{id}'.", false);

                // Verificar matrícula duplicada
                var todasRes = _estudianteService.ObtenerTodas();
                if (todasRes.Success && todasRes.Data != null)
                {
                    var matriculaDuplicada = todasRes.Data
                        .OfType<Estudiante>()
                        .Any(e => string.Equals(e.Matricula, mat, StringComparison.OrdinalIgnoreCase));
                    if (matriculaDuplicada)
                        return Result<bool>.Fail($"Ya existe un estudiante con la matrícula '{mat}'.", false);
                }

                Estudiante est;
                if (tipo == "Pregrado")
                {
                    var pre = new EstudiantePregrado(
                        id, nombre, fechaNacimiento, "", "", mat,
                        DateTime.Now, 0.0, "Activo", acudiente, 1, 0, programa);

                    if (!string.IsNullOrWhiteSpace(colegio) && añoBach > 0)
                        pre.EstablecerDatosBachillerato(colegio, añoBach, tituloDiploma);

                    est = pre;
                }
                else
                {
                    var pos = new EstudiantePosgrado(
                        id, nombre, fechaNacimiento, "", "", mat,
                        DateTime.Now, 0.0, "Activo", acudiente, programa, 1, false);

                    if (!string.IsNullOrWhiteSpace(instPrevia) && añoGradPrevio > 0)
                        pos.AgregarEstudioPrevio(instPrevia, tituloObtenido, añoGradPrevio);

                    est = pos;
                }

                var result = _estudianteService.AdicionarPersona(est);
                return result.Success
                    ? Result<bool>.Ok(true, "Estudiante registrado correctamente.")
                    : Result<bool>.Fail(result.Message, false);
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error: {ex.Message}", false);
            }
        }

        public HistoriaAcademicaViewModel ObtenerHistoriaAcademica(string idEstudiante)
        {
            var vm = new HistoriaAcademicaViewModel { IdEstudiante = idEstudiante };
            var historiaRes = _estudianteService.ObtenerHistoria(idEstudiante);
            if (historiaRes.Success && historiaRes.Data != null)
            {
                var h = historiaRes.Data;
                vm.PromedioGeneral      = h.PromedioGeneral;
                vm.AsignaturasAprobadas = h.AsignaturasAprobadas.ToList();
                vm.AsignaturasReprobadas = h.AsignaturasReprobadas.ToList();
                vm.Evaluaciones = _evalLocal
                    .Where(e => e.Estudiante?.Id == idEstudiante)
                    .Select(EvaluacionViewModel.FromDomain)
                    .ToList();
            }
            return vm;
        }

        public Result<bool> EliminarEstudiante(string id)
        {
            // Eliminar evaluaciones del estudiante localmente para reflejar UI
            _evalLocal.RemoveAll(e => e.Estudiante?.Id == id);
            return _estudianteService.EliminarPersona(id);
        }

        // ── Docentes ──────────────────────────────────────────────

        public List<DocenteViewModel> ObtenerDocentes()
        {
            var res = _docenteService.ObtenerTodas();
            if (!res.Success || res.Data == null) return new();
            return res.Data.Select(p =>
            {
                var vm = DocenteViewModel.FromDomain(p);
                vm.AsignaturasAsignadas = _asignaturaService
                    .ObtenerPorDocente(vm.Id)
                    .Select(a => a.Nombre)
                    .ToList();
                return vm;
            }).ToList();
        }

        public Result<bool> RegistrarDocente(
            string   id,
            string   nombre,
            string   area,
            DateTime fechaNacimiento)
        {
            try
            {
                var nombreVal = ValidadorAcademico.ValidarNombreSoloLetras(nombre);
                if (!nombreVal.valido)
                    return Result<bool>.Fail(nombreVal.mensaje, false);

                var doc = new Docente(id, nombre, fechaNacimiento, "", "",
                    id, area, area, 5_000_000, true, new List<string> { "Maestría" }, area);
                _docenteService.AdicionarPersona(doc);
                return Result<bool>.Ok(true, "Docente registrado.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error: {ex.Message}", false);
            }
        }

        // Sobrecarga compatible con código anterior (sin fechaNacimiento)
        public Result<bool> RegistrarDocente(string id, string nombre, string area)
            => RegistrarDocente(id, nombre, area, new DateTime(1980, 1, 1));

        public Result<bool> EliminarDocente(string id)
        {
            return _docenteService.EliminarPersona(id);
        }

        // ── Asignaturas ───────────────────────────────────────────

        public List<AsignaturaViewModel> ObtenerAsignaturas()
            => _asignaturaService.ObtenerTodas().Select(AsignaturaViewModel.FromDomain).ToList();

        public Result<bool> RegistrarAsignatura(string codigo, string nombre, string area, string idDocente)
        {
            try
            {
                var asig = new Asignatura(codigo, nombre, 3, area, 3, 1, "Primero", new List<Docente>());
                var docenteRes = _docenteService.ObtenerPersona(idDocente);
                if (docenteRes.Success && docenteRes.Data is Docente doc)
                    asig.DocentesAsignados.Add(doc);

                _asignaturaService.AdicionarAsignatura(asig);
                return Result<bool>.Ok(true, "Asignatura registrada.");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error: {ex.Message}", false);
            }
        }

        public Result<bool> EliminarAsignatura(string codigo)
        {
            return _asignaturaService.EliminarAsignatura(codigo);
        }

        // ── Evaluaciones ──────────────────────────────────────────

        public List<EvaluacionViewModel> ObtenerEvaluaciones()
            => _evalLocal.Select(EvaluacionViewModel.FromDomain).ToList();

        public List<EvaluacionViewModel> ObtenerEvaluacionesPorEstudiante(string idEstudiante)
            => _evalLocal
                .Where(e => e.Estudiante?.Id == idEstudiante)
                .Select(EvaluacionViewModel.FromDomain).ToList();

        public Result<bool> RegistrarEvaluacion(string idEstudiante, string codigoAsignatura, double nota, string periodo)
        {
            if (nota < 0 || nota > 5)
                return Result<bool>.Fail("La nota debe estar entre 0.0 y 5.0.", false);

            var estRes = _estudianteService.ObtenerPersona(idEstudiante);
            if (!estRes.Success || estRes.Data is not Estudiante estudiante)
                return Result<bool>.Fail("Estudiante no encontrado.", false);

            var asignatura = _asignaturaService.ObtenerPorCodigo(codigoAsignatura);
            if (asignatura == null)
                return Result<bool>.Fail("Asignatura no encontrada.", false);

            var id = Guid.NewGuid().ToString("N")[..8].ToUpper();
            var ev = new Evaluacion(id, estudiante, asignatura, DateTime.Now, nota, "Normal", periodo);

            var res = _universidadService.RegistrarEvaluacion(ev);
            if (res.Success) _evalLocal.Add(ev);
            return res;
        }

        public Result<bool> EliminarEvaluacion(string id)
        {
            var eval = _evalLocal.FirstOrDefault(e => e.Id == id);
            if (eval != null)
            {
                _evalLocal.Remove(eval);
                return _universidadService.EliminarEvaluacion(id);
            }
            return Result<bool>.Fail("Evaluación no encontrada localmente", false);
        }

        public Result<bool> CancelarMateria(string idEstudiante, string codigoAsignatura, string motivo)
        {
            var estRes = _estudianteService.ObtenerPersona(idEstudiante);
            if (!estRes.Success || estRes.Data is not Estudiante estudiante)
                return Result<bool>.Fail("Estudiante no encontrado.", false);

            var asignatura = _asignaturaService.ObtenerPorCodigo(codigoAsignatura);
            if (asignatura == null)
                return Result<bool>.Fail("Asignatura no encontrada.", false);

            var id = Guid.NewGuid().ToString("N")[..8].ToUpper();
            var cancelacion = new CancelacionMateria(id, estudiante, asignatura, DateTime.Now, motivo, "Pendiente");

            var res = _universidadService.CancelarMateria(cancelacion);
            if (res.Success)
            {
                _evalLocal.RemoveAll(e => e.Estudiante.Id == idEstudiante && e.Asignatura.Codigo == codigoAsignatura);
                _evaluacionService.RemoverEvaluacion(idEstudiante, codigoAsignatura);
                var nuevoPromedio = _evaluacionService.CalcularPromedioEstudiante(idEstudiante);
                _estudianteService.ActualizarPromedio(idEstudiante, nuevoPromedio.Data);
            }
            return res;
        }

        // ── Graduación ────────────────────────────────────────────

        public Result<bool> GraduarEstudiante(string idEstudiante)
            => _universidadService.GraduarEstudiante(idEstudiante);

        public string ObtenerRequisitosGrado(string idEstudiante)
        {
            var res = _estudianteService.ObtenerPersona(idEstudiante);
            if (!res.Success || res.Data is not Estudiante est)
                return "Estudiante no encontrado.";
            return est.ObtenerRequisitosDeGrado();
        }

        /// <summary>Registra el requisito de grado de un EstudiantePregrado.</summary>
        public Result<bool> RegistrarRequisitosGradoPregrado(string idEstudiante, string tipo, string monitor, double nota)
        {
            try
            {
                var res = _estudianteService.ObtenerPersona(idEstudiante);
                if (!res.Success || res.Data is not EstudiantePregrado pre)
                    return Result<bool>.Fail("Estudiante de pregrado no encontrado.", false);
                pre.EstablecerRequisitosGrado(tipo, monitor, nota);
                return Result<bool>.Ok(true, "Requisito de grado registrado.");
            }
            catch (Exception ex) { return Result<bool>.Fail(ex.Message, false); }
        }

        /// <summary>Registra la tesis de un EstudiantePosgrado.</summary>
        public Result<bool> RegistrarTesisPosgrado(string idEstudiante, string titulo, string director,
            string eval1, string eval2, string nota, DateTime fecha)
        {
            try
            {
                var res = _estudianteService.ObtenerPersona(idEstudiante);
                if (!res.Success || res.Data is not EstudiantePosgrado pos)
                    return Result<bool>.Fail("Estudiante de posgrado no encontrado.", false);
                pos.RegistrarTesis(titulo, director, eval1, eval2, nota, fecha);
                return Result<bool>.Ok(true, "Tesis registrada correctamente.");
            }
            catch (Exception ex) { return Result<bool>.Fail(ex.Message, false); }
        }

        // ── Eventos y Análisis ────────────────────────────────────

        public Result<List<Estudiante>> EjecutarAnalisisPermanencia()
        {
            var res = _universidadService.ObtenerEstudiantesEnRiesgo();
            if (res.Success && res.Data != null)
            {
                foreach (var est in res.Data)
                {
                    var ev = new EventoAlertaPermanencia(
                        Guid.NewGuid().ToString("N")[..8], DateTime.Now,
                        est.Id, $"Rendimiento bajo detectado para {est.Nombre}",
                        est.Promedio, 1, "Requiere acompañamiento académico");
                    _gestorEventos.PublicarEvento(ev);
                }
            }
            return res;
        }

        /// <summary>
        /// Ejecuta el análisis completo con generación de EventoAlertaConOportunidad / EventoAlertaSinOportunidad.
        /// </summary>
        public Result<List<EventoAcademicoBase>> EjecutarAnalisisConEventos()
            => _universidadService.AnalizarEvaluacionesConEventos();

        public List<EventoAcademicoBase> ObtenerHistorialEventos()
        {
            var res = _gestorEventos.ObtenerHistorial();
            return res.Success && res.Data != null ? res.Data : new List<EventoAcademicoBase>();
        }

        // ── Reportes y Dashboard ──────────────────────────────────

        public ReporteAcademicoViewModel GenerarReporteGlobal()
        {
            var reporte = new ReporteAcademicoViewModel();

            var estudiantesRes = _estudianteService.ObtenerTodas();
            if (estudiantesRes.Success && estudiantesRes.Data != null)
            {
                var ests = estudiantesRes.Data.OfType<Estudiante>().ToList();
                reporte.EstudiantesAprobados       = ests.Count(e => e.Promedio >= 3.0);
                reporte.EstudiantesPerdidos        = ests.Count(e => e.Promedio < 3.0);
                reporte.PromedioGeneralUniversidad = ests.Any() ? Math.Round(ests.Average(e => e.Promedio), 2) : 0;
            }

            // Estadísticas de evaluaciones via AnalizadorEvaluacionService
            var aprobRes = _analizador.ContarAprobadas();
            if (aprobRes.Success) reporte.EvaluacionesAprobadas = aprobRes.Data;

            var reprobRes = _analizador.ContarReprobadas();
            if (reprobRes.Success) reporte.EvaluacionesReprobadas = reprobRes.Data;

            var masPerdida = _analizador.ObtenerAsignaturaMasPerdida();
            if (masPerdida.Success) reporte.AsignaturaMasPerdida = masPerdida.Data;

            var mejorDesempeno = _analizador.ObtenerAsignaturaMejorDesempeno();
            if (mejorDesempeno.Success) reporte.AsignaturaMejorPromedio = mejorDesempeno.Data;

            // Asignatura peor desempeño (la de menor promedio con evaluaciones)
            var evalRes = _evaluacionService.ObtenerTodas();
            if (evalRes.Success && evalRes.Data.Any())
            {
                var peor = evalRes.Data
                    .GroupBy(e => e.Asignatura.Nombre)
                    .Select(g => new { Nombre = g.Key, Prom = g.Average(e => e.NotaFinal) })
                    .OrderBy(x => x.Prom).First();
                reporte.AsignaturaPeorPromedio = $"{peor.Nombre} ({peor.Prom:F1})";
            }

            var riesgoRes = _universidadService.ObtenerEstudiantesEnRiesgo();
            if (riesgoRes.Success) reporte.EstudiantesEnRiesgoCount = riesgoRes.Data.Count;

            return reporte;
        }

        public DashboardStats ObtenerEstadisticasDashboard()
        {
            double promedio = _evalLocal.Any() ? Math.Round(_evalLocal.Average(e => e.NotaFinal), 1) : 0.0;
            return new DashboardStats
            {
                TotalEstudiantes  = ObtenerEstudiantes().Count,
                TotalDocentes     = ObtenerDocentes().Count,
                TotalAsignaturas  = ObtenerAsignaturas().Count,
                TotalEvaluaciones = _evalLocal.Count,
                PromedioGeneral   = promedio,
                EvaluacionesHoy   = _evalLocal.Count(e => e.Fecha.Date == DateTime.Today),
                AlertasActivas    = 0
            };
        }

        // ── Siembra de datos de demostración ─────────────────────

        private void SembrarDatosDemostracion()
        {
            var check = _estudianteService.ObtenerTodas();
            if (check.Success && check.Data?.Count > 0) return;

            // Docentes
            var doc1 = new Docente("D001", "Carlos Ramírez", new DateTime(1978, 3, 10),
                "carlos@universidad.edu", "3001234561", "EMP001", "Sistemas", "Programación", 5_200_000, true,
                new List<string> { "MSc. Ingeniería de Software", "Esp. POO" }, "Programación");
            var doc2 = new Docente("D002", "María López", new DateTime(1982, 6, 20),
                "maria@universidad.edu", "3001234562", "EMP002", "Sistemas", "Bases de Datos", 5_400_000, true,
                new List<string> { "PhD. Ciencias de Computación", "MSc. BD" }, "Bases de Datos");
            var doc3 = new Docente("D003", "Andrés Herrera", new DateTime(1975, 11, 5),
                "andres@universidad.edu", "3001234563", "EMP003", "Matemáticas", "Cálculo", 4_900_000, true,
                new List<string> { "MSc. Matemáticas Aplicadas" }, "Matemáticas");

            _docenteService.AdicionarPersona(doc1);
            _docenteService.AdicionarPersona(doc2);
            _docenteService.AdicionarPersona(doc3);

            // Asignaturas
            var asig1 = new Asignatura("CS101", "Programación Orientada a Objetos", 4, "Sistemas",   3, 1, "Primero", new List<Docente> { doc1 });
            var asig2 = new Asignatura("CS201", "Bases de Datos",                   3, "Sistemas",   2, 1, "Tercero", new List<Docente> { doc2 });
            var asig3 = new Asignatura("MA101", "Cálculo Diferencial",              4, "Matemáticas",4, 0, "Primero", new List<Docente> { doc3 });
            var asig4 = new Asignatura("CS301", "Estructuras de Datos",             4, "Sistemas",   3, 1, "Segundo", new List<Docente> { doc1, doc2 });
            _asignaturaService.AdicionarAsignatura(asig1);
            _asignaturaService.AdicionarAsignatura(asig2);
            _asignaturaService.AdicionarAsignatura(asig3);
            _asignaturaService.AdicionarAsignatura(asig4);

            // Estudiantes pregrado
            var est1 = new EstudiantePregrado("E001", "Juan Pérez",  new DateTime(2002, 5, 15),
                "juan@universidad.edu", "3101234567", "MAT001", new DateTime(2022, 1, 15), 3.8, "Activo", "María Pérez", 4, 72, "Ingeniería de Sistemas");
            var est2 = new EstudiantePregrado("E002", "Ana García",  new DateTime(2003, 8, 22),
                "ana@universidad.edu",  "3101234568", "MAT002", new DateTime(2023, 1, 15), 4.2, "Activo", "Luis García", 2, 36, "Ingeniería de Sistemas");
            var est3 = new EstudiantePregrado("E003", "Carlos Mora", new DateTime(2001, 3, 10),
                "carlos.m@universidad.edu", "3101234569", "MAT003", new DateTime(2021, 1, 15), 2.7, "En Riesgo", "Rosa Mora", 6, 108, "Ingeniería de Sistemas");

            // Datos de bachillerato
            est1.EstablecerDatosBachillerato("Colegio San Ignacio", 2020, "Bachiller Académico");
            est2.EstablecerDatosBachillerato("Instituto Nacional",   2021, "Bachiller Técnico");
            est3.EstablecerDatosBachillerato("Colegio Los Andes",    2019, "Bachiller Académico");

            // Requisitos de grado pregrado
            est1.EstablecerRequisitosGrado("Práctica Empresarial",      "Ing. Pedro Gómez", 4.8);
            est2.EstablecerRequisitosGrado("Pasantía Investigativa",    "Dra. Laura Ruiz",  4.2);
            est3.EstablecerRequisitosGrado("Plan de Negocios",          "Carlos Torres",    2.5);

            // Estudiantes posgrado
            var est4 = new EstudiantePosgrado("E004", "Sofía Martínez", new DateTime(1995, 12, 1),
                "sofia@universidad.edu", "3201234570", "POS001", new DateTime(2023, 8, 1), 4.5, "Activo", "Pedro Martínez", "Maestría Ing. Software", 3, false);
            var est5 = new EstudiantePosgrado("E005", "Diego Vargas",   new DateTime(1992, 7, 18),
                "diego@universidad.edu", "3201234571", "POS002", new DateTime(2022, 1, 1),  3.9, "Activo", "Carmen Vargas",  "Maestría Ciencias de Datos", 5, true);

            // Estudios previos
            est4.AgregarEstudioPrevio("Universidad Nacional", "Ingeniería de Sistemas", 2018);
            est5.AgregarEstudioPrevio("Universidad de los Andes", "Matemáticas", 2015);
            est5.AgregarEstudioPrevio("Universidad Javeriana",    "Estadística",  2017);

            // Tesis
            est4.RegistrarTesis("IA en la Salud",   "Dr. Martínez", "Dra. Silva", "Dr. Gómez", "Laureada", DateTime.Now.AddDays(-10));
            est5.RegistrarTesis("Big Data Urbano",  "Dr. Ríos",     "Dr. Paz",    "Dra. Mora", "Aprobó",   DateTime.Now.AddDays(-5));

            _estudianteService.AdicionarPersona(est1);
            _estudianteService.AdicionarPersona(est2);
            _estudianteService.AdicionarPersona(est3);
            _estudianteService.AdicionarPersona(est4);
            _estudianteService.AdicionarPersona(est5);

            // Evaluaciones demo
            var demos = new[]
            {
                new Evaluacion("EV001", est1, asig1, DateTime.Now, 4.3, "Parcial", "2026-1"),
                new Evaluacion("EV002", est1, asig3, DateTime.Now, 3.7, "Parcial", "2026-1"),
                new Evaluacion("EV003", est2, asig1, DateTime.Now, 4.8, "Parcial", "2026-1"),
                new Evaluacion("EV004", est3, asig2, DateTime.Now, 2.5, "Parcial", "2026-1"),
                new Evaluacion("EV005", est4, asig4, DateTime.Now, 4.1, "Final",   "2026-1"),
                new Evaluacion("EV006", est5, asig4, DateTime.Now, 3.9, "Final",   "2026-1"),
            };
            foreach (var ev in demos)
            {
                _evaluacionService.RegistrarEvaluacion(ev);
                _evalLocal.Add(ev);
                _estudianteService.AdicionarEvaluacionAHistoria(ev.Estudiante.Id, ev);
            }
        }
    }
}
