namespace SistemaAcademico.Interfaces
{
    using Domain.Models;

    public interface IReglasNegocioAcademicas
    {
        double ObtenerNotaMinima();
        double ObtenerNotaMaxima();
        int ObtenerEdadMinima();
    }
}
