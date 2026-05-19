namespace SistemaAcademico.Services
{
    using Domain.Models;
    using Interfaces;

    public abstract class PersonaService : IPersonaService
    {
        protected List<Persona> Personas { get; set; }

        protected PersonaService()
        {
            Personas = new List<Persona>();
        }

        public virtual Result<Persona> ObtenerPersona(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return Result<Persona>.Fail("ID no puede estar vacío", null);

                var persona = Personas.FirstOrDefault(p => p.Id == id);
                if (persona == null)
                    return Result<Persona>.Fail($"Persona con ID {id} no encontrada", null);

                return Result<Persona>.Ok(persona);
            }
            catch (Exception ex)
            {
                return Result<Persona>.Fail($"Error al obtener persona: {ex.Message}", null);
            }
        }

        public virtual Result<List<Persona>> ObtenerTodas()
        {
            try
            {
                if (Personas.Count == 0)
                    return Result<List<Persona>>.Ok(new List<Persona>(), "No hay personas registradas");

                return Result<List<Persona>>.Ok(Personas.ToList());
            }
            catch (Exception ex)
            {
                return Result<List<Persona>>.Fail($"Error al obtener todas las personas: {ex.Message}", new List<Persona>());
            }
        }

        public virtual Result<bool> EliminarPersona(string id)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(id))
                    return Result<bool>.Fail("ID no puede estar vacío", false);

                var persona = Personas.FirstOrDefault(p => p.Id == id);
                if (persona == null)
                    return Result<bool>.Fail($"Persona con ID {id} no encontrada", false);

                Personas.Remove(persona);
                return Result<bool>.Ok(true, "Persona eliminada exitosamente");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al eliminar persona: {ex.Message}", false);
            }
        }

        public virtual Result<bool> AdicionarPersona(Persona persona)
        {
            try
            {
                if (persona == null)
                    return Result<bool>.Fail("Persona no puede ser nula", false);

                if (Personas.Any(p => p.Id == persona.Id))
                    return Result<bool>.Fail($"Ya existe una persona con ID {persona.Id}", false);

                Personas.Add(persona);
                return Result<bool>.Ok(true, "Persona agregada exitosamente");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al agregar persona: {ex.Message}", false);
            }
        }
    }
}
