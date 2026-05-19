namespace SistemaAcademico.Services
{
    using Domain.Models;
    using Interfaces;

    public class DocenteService : PersonaService, IDocenteService
    {
        public DocenteService()
        {
        }

        public override Result<bool> AdicionarPersona(Persona persona)
        {
            if (persona is not Docente)
                return Result<bool>.Fail("La persona debe ser un Docente", false);

            return base.AdicionarPersona(persona);
        }

        public Result<Docente> ObtenerDocente(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return Result<Docente>.Fail("ID no puede estar vacío", null);

                var docente = Personas.FirstOrDefault(p => p.Id == id) as Docente;
                if (docente == null)
                    return Result<Docente>.Fail($"Docente con ID {id} no encontrado", null);

                return Result<Docente>.Ok(docente);
            }
            catch (Exception ex)
            {
                return Result<Docente>.Fail($"Error al obtener docente: {ex.Message}", null);
            }
        }

        public Result<List<Docente>> ObtenerDocentesPorDepartamento(string departamento)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(departamento))
                    return Result<List<Docente>>.Fail("Departamento no puede estar vacío", new List<Docente>());

                var docentes = Personas.OfType<Docente>()
                    .Where(d => d.Departamento == departamento)
                    .ToList();

                return Result<List<Docente>>.Ok(docentes);
            }
            catch (Exception ex)
            {
                return Result<List<Docente>>.Fail($"Error al obtener docentes por departamento: {ex.Message}", new List<Docente>());
            }
        }

        public Result<bool> ActivarDocente(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return Result<bool>.Fail("ID no puede estar vacío", false);

                var docente = Personas.FirstOrDefault(p => p.Id == id) as Docente;
                if (docente == null)
                    return Result<bool>.Fail($"Docente con ID {id} no encontrado", false);

                docente.ActivarDocente();
                return Result<bool>.Ok(true, "Docente activado exitosamente");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al activar docente: {ex.Message}", false);
            }
        }

        public Result<bool> DesactivarDocente(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return Result<bool>.Fail("ID no puede estar vacío", false);

                var docente = Personas.FirstOrDefault(p => p.Id == id) as Docente;
                if (docente == null)
                    return Result<bool>.Fail($"Docente con ID {id} no encontrado", false);

                docente.DesactivarDocente();
                return Result<bool>.Ok(true, "Docente desactivado exitosamente");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al desactivar docente: {ex.Message}", false);
            }
        }

        public Result<List<Docente>> ObtenerDocentesActivos()
        {
            try
            {
                var activos = Personas.OfType<Docente>()
                    .Where(d => d.Activo)
                    .ToList();

                return Result<List<Docente>>.Ok(activos);
            }
            catch (Exception ex)
            {
                return Result<List<Docente>>.Fail($"Error al obtener docentes activos: {ex.Message}", new List<Docente>());
            }
        }
    }
}
