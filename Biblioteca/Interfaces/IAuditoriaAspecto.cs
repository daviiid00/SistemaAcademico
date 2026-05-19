namespace SistemaAcademico.Interfaces
{
    using Domain.Models;

    public interface IAuditoriaAspecto
    {
        Result<bool> RegistrarAcceso(string usuario, string operacion, DateTime fecha);
        Result<List<(string Usuario, string Operacion, DateTime Fecha)>> ObtenerRegistros();
    }
}
