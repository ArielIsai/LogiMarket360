namespace LogiMarket.Domain.Entregas;

public interface ICalculadorPlazo
{
    TipoEntrega Familia { get; }
    int DiasEstimados { get; }
    DateOnly FechaEstimada(DateOnly desde);
}
