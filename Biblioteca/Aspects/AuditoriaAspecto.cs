namespace SistemaAcademico.Aspects
{
    using Domain.Models;
    using Interfaces;

    public class AuditoriaAspecto : IAuditoriaAspecto
    {
        private readonly List<(string Usuario, string Operacion, DateTime Fecha)> _registros;

        public AuditoriaAspecto()
        {
            _registros = new List<(string, string, DateTime)>();
        }

        public Result<bool> RegistrarAcceso(string usuario, string operacion, DateTime fecha)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(usuario) || string.IsNullOrWhiteSpace(operacion))
                    return Result<bool>.Fail("Usuario y operación son requeridos", false);

                _registros.Add((usuario, operacion, fecha));
                return Result<bool>.Ok(true, "Acceso registrado en auditoría");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al registrar en auditoría: {ex.Message}", false);
            }
        }

        public Result<List<(string Usuario, string Operacion, DateTime Fecha)>> ObtenerRegistros()
        {
            try
            {
                return Result<List<(string, string, DateTime)>>.Ok(_registros.OrderByDescending(r => r.Fecha).ToList());
            }
            catch (Exception ex)
            {
                return Result<List<(string, string, DateTime)>>.Fail($"Error al obtener registros: {ex.Message}", new List<(string, string, DateTime)>());
            }
        }
    }
}
