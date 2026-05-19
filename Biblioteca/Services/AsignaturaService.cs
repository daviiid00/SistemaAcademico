namespace SistemaAcademico.Services
{
    using Domain.Models;

    /// <summary>
    /// Servicio de gestión de asignaturas del sistema académico.
    /// </summary>
    public class AsignaturaService
    {
        private readonly List<Asignatura> _asignaturas = new();

        public void AdicionarAsignatura(Asignatura asignatura)
        {
            if (asignatura == null) return;
            if (!_asignaturas.Any(a => a.Codigo == asignatura.Codigo))
                _asignaturas.Add(asignatura);
        }

        public Result<bool> EliminarAsignatura(string codigo)
        {
            var asignatura = _asignaturas.FirstOrDefault(a => a.Codigo == codigo);
            if (asignatura == null)
                return Result<bool>.Fail($"Asignatura con código {codigo} no encontrada", false);

            _asignaturas.Remove(asignatura);
            return Result<bool>.Ok(true, "Asignatura eliminada exitosamente");
        }

        public List<Asignatura> ObtenerTodas() => new(_asignaturas);

        public Asignatura? ObtenerPorCodigo(string codigo)
            => _asignaturas.FirstOrDefault(a => a.Codigo == codigo);

        public List<Asignatura> ObtenerPorGrado(string grado)
            => _asignaturas.Where(a => a.Grado == grado).ToList();

        public List<Asignatura> ObtenerPorDocente(string idDocente)
            => _asignaturas
                .Where(a => a.DocentesAsignados.Any(d => d.Id == idDocente))
                .ToList();
    }
}
