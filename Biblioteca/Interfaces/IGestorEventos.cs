namespace SistemaAcademico.Interfaces
{
    using Domain.Models;

    public interface IGestorEventos
    {
        Result<bool> PublicarEvento(EventoAcademicoBase evento);
        Result<bool> Suscribirse(IObservadorAcademico observador);
        Result<bool> Desuscribirse(IObservadorAcademico observador);
        Result<List<EventoAcademicoBase>> ObtenerHistorial();
        Result<List<EventoAcademicoBase>> ObtenerEventosCriticos();
    }
}
