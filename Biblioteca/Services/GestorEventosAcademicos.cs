namespace SistemaAcademico.Services
{
    using Domain.Models;
    using Interfaces;

    public class GestorEventosAcademicos : IGestorEventos
    {
        private readonly List<IObservadorAcademico> _observadores;
        private readonly List<EventoAcademicoBase> _historial;

        public GestorEventosAcademicos()
        {
            _observadores = new List<IObservadorAcademico>();
            _historial = new List<EventoAcademicoBase>();
        }

        public Result<bool> PublicarEvento(EventoAcademicoBase evento)
        {
            try
            {
                if (evento == null)
                    return Result<bool>.Fail("Evento no puede ser nulo", false);

                _historial.Add(evento);

                foreach (var observador in _observadores.ToList())
                {
                    var resultado = observador.Notificar(evento);
                    if (!resultado.Success)
                    {
                        return Result<bool>.Fail($"Error notificando observador: {resultado.Message}", false);
                    }
                }

                return Result<bool>.Ok(true, "Evento publicado exitosamente");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al publicar evento: {ex.Message}", false);
            }
        }

        public Result<bool> Suscribirse(IObservadorAcademico observador)
        {
            try
            {
                if (observador == null)
                    return Result<bool>.Fail("Observador no puede ser nulo", false);

                if (_observadores.Contains(observador))
                    return Result<bool>.Fail("Observador ya está suscrito", false);

                _observadores.Add(observador);
                return Result<bool>.Ok(true, "Observador suscrito exitosamente");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al suscribirse: {ex.Message}", false);
            }
        }

        public Result<bool> Desuscribirse(IObservadorAcademico observador)
        {
            try
            {
                if (observador == null)
                    return Result<bool>.Fail("Observador no puede ser nulo", false);

                if (!_observadores.Contains(observador))
                    return Result<bool>.Fail("Observador no está suscrito", false);

                _observadores.Remove(observador);
                return Result<bool>.Ok(true, "Observador desuscrito exitosamente");
            }
            catch (Exception ex)
            {
                return Result<bool>.Fail($"Error al desuscribirse: {ex.Message}", false);
            }
        }

        public Result<List<EventoAcademicoBase>> ObtenerHistorial()
        {
            try
            {
                return Result<List<EventoAcademicoBase>>.Ok(_historial.OrderByDescending(e => e.FechaOcurrencia).ToList());
            }
            catch (Exception ex)
            {
                return Result<List<EventoAcademicoBase>>.Fail($"Error al obtener historial: {ex.Message}", new List<EventoAcademicoBase>());
            }
        }

        public Result<List<EventoAcademicoBase>> ObtenerEventosCriticos()
        {
            try
            {
                var criticos = _historial.Where(e => e.EsCritico()).OrderByDescending(e => e.FechaOcurrencia).ToList();
                return Result<List<EventoAcademicoBase>>.Ok(criticos);
            }
            catch (Exception ex)
            {
                return Result<List<EventoAcademicoBase>>.Fail($"Error al obtener eventos críticos: {ex.Message}", new List<EventoAcademicoBase>());
            }
        }
    }
}
