namespace SistemaAcademico.Interfaces
{
    using Domain.Models;

    public interface IObservadorAcademico
    {
        Result<bool> Notificar(EventoAcademicoBase evento);
    }
}
