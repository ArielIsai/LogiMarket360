namespace LogiMarket.Domain.Entregas;

public interface ICalculadorTarifa
{
    TipoEntrega Familia { get; }
    decimal Calcular();
}
