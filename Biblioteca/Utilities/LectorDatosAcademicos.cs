namespace SistemaAcademico.Utilities
{
    using Domain.Models;
    using Services;

    public class LectorDatosAcademicos
    {
        public static Result<bool> CargarEstudiantesDesdeCsv(EstudianteService estudianteService, string rutaArchivo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rutaArchivo))
                    return Result<bool>.Fail("Ruta de archivo no puede estar vacía", false);

                if (!File.Exists(rutaArchivo))
                    return Result<bool>.Fail($"Archivo no encontrado: {rutaArchivo}", false);

                var lineas = File.ReadAllLines(rutaArchivo);
                if (lineas.Length == 0)
                    return Result<bool>.Fail("Archivo vacío", false);

                foreach (var linea in lineas.Skip(1))
                {
                    if (string.IsNullOrWhiteSpace(linea))
                        continue;

                    var partes = linea.Split(',');
                    if (partes.Length < 8)
                        continue;

                    try
                    {
                        string tipo = partes[0].Trim();
                        string id = partes[1].Trim();
                        string nombre = partes[2].Trim();
                        DateTime.TryParse(partes[3].Trim(), out DateTime fechaNacimiento);
                        string email = partes[4].Trim();
                        string telefono = partes[5].Trim();
                        string matricula = partes[6].Trim();
                        DateTime.TryParse(partes[7].Trim(), out DateTime fechaIngreso);

                        if (tipo == "Pregrado" && partes.Length >= 11)
                        {
                            if (int.TryParse(partes[8].Trim(), out int semestre) &&
                                int.TryParse(partes[9].Trim(), out int creditos))
                            {
                                string programa = partes[10].Trim();
                                var estudiante = new EstudiantePregrado(
                                    id, nombre, fechaNacimiento, email, telefono,
                                    matricula, fechaIngreso, 3.5, "Activo", "Sin Acudiente",
                                    semestre, creditos, programa
                                );
                                estudianteService.AdicionarPersona(estudiante);
                            }
                        }
                        else if (tipo == "Posgrado" && partes.Length >= 11)
                        {
                            string maestria = partes[8].Trim();
                            if (int.TryParse(partes[9].Trim(), out int semestres))
                            {
                                bool.TryParse(partes[10].Trim(), out bool tesisPresentada);
                                var estudiante = new EstudiantePosgrado(
                                    id, nombre, fechaNacimiento, email, telefono,
                                    matricula, fechaIngreso, 4.0, "Activo", "Sin Acudiente",
                                    maestria, semestres, tesisPresentada
                                );
                                estudianteService.AdicionarPersona(estudiante);
                            }
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }

                return Result<bool>.Ok(true, "Estudiantes cargados exitosamente");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al cargar estudiantes: {ex.Message}", false);
            }
        }

        public static Result<bool> CargarDocentesDesdeCsv(DocenteService docenteService, string rutaArchivo)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(rutaArchivo))
                    return Result<bool>.Fail("Ruta de archivo no puede estar vacía", false);

                if (!File.Exists(rutaArchivo))
                    return Result<bool>.Fail($"Archivo no encontrado: {rutaArchivo}", false);

                var lineas = File.ReadAllLines(rutaArchivo);
                if (lineas.Length == 0)
                    return Result<bool>.Fail("Archivo vacío", false);

                foreach (var linea in lineas.Skip(1))
                {
                    if (string.IsNullOrWhiteSpace(linea))
                        continue;

                    var partes = linea.Split(',');
                    if (partes.Length < 10)
                        continue;

                    try
                    {
                        string id = partes[0].Trim();
                        string nombre = partes[1].Trim();
                        DateTime.TryParse(partes[2].Trim(), out DateTime fechaNacimiento);
                        string email = partes[3].Trim();
                        string telefono = partes[4].Trim();
                        string numeroEmpleado = partes[5].Trim();
                        string departamento = partes[6].Trim();
                        string especialidad = partes[7].Trim();
                        double.TryParse(partes[8].Trim(), out double salario);
                        bool.TryParse(partes[9].Trim(), out bool activo);

                        var docente = new Docente(
                            id, nombre, fechaNacimiento, email, telefono,
                            numeroEmpleado, departamento, especialidad, salario, activo,
                            new List<string> { "Especialidad" }, especialidad
                        );
                        docenteService.AdicionarPersona(docente);
                    }
                    catch
                    {
                        continue;
                    }
                }

                return Result<bool>.Ok(true, "Docentes cargados exitosamente");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al cargar docentes: {ex.Message}", false);
            }
        }

        public static Result<bool> CrearArchivoEstudiantesSample(string rutaArchivo)
        {
            try
            {
                var lineas = new List<string>
                {
                    "Tipo,Id,Nombre,FechaNacimiento,Email,Telefono,Matricula,FechaIngreso,Parametro1,Parametro2,Parametro3",
                    "Pregrado,E001,Juan Perez,1995-05-15,juan@universidad.edu,3001234567,MAT001,2020-01-15,2,45,Ingenieria Sistemas",
                    "Pregrado,E002,Maria Garcia,1996-03-20,maria@universidad.edu,3001234568,MAT002,2020-01-15,3,60,Ingenieria Sistemas",
                    "Posgrado,E003,Carlos Lopez,1990-07-10,carlos@universidad.edu,3001234569,MAT003,2022-01-15,Maestria Sistemas,2,true",
                    "Posgrado,E004,Ana Martinez,1992-11-25,ana@universidad.edu,3001234570,MAT004,2022-09-01,Maestria Redes,1,false"
                };

                File.WriteAllLines(rutaArchivo, lineas);
                return Result<bool>.Ok(true, $"Archivo de muestra creado: {rutaArchivo}");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al crear archivo: {ex.Message}", false);
            }
        }

        public static Result<bool> CrearArchivoDocentesSample(string rutaArchivo)
        {
            try
            {
                var lineas = new List<string>
                {
                    "Id,Nombre,FechaNacimiento,Email,Telefono,NumeroEmpleado,Departamento,Especialidad,Salario,Activo",
                    "D001,Prof. Roberto,1975-03-15,roberto@universidad.edu,3101234560,EMP001,Sistemas,Programacion,5000000,true",
                    "D002,Prof. Elena,1980-06-20,elena@universidad.edu,3101234561,EMP002,Sistemas,BaseDatos,5200000,true",
                    "D003,Prof. Miguel,1978-09-10,miguel@universidad.edu,3101234562,EMP003,Redes,Telecomunicaciones,5100000,true",
                    "D004,Prof. Lucia,1982-12-05,lucia@universidad.edu,3101234563,EMP004,Redes,Seguridad,5300000,false"
                };

                File.WriteAllLines(rutaArchivo, lineas);
                return Result<bool>.Ok(true, $"Archivo de muestra creado: {rutaArchivo}");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al crear archivo: {ex.Message}", false);
            }
        }
    }
}
