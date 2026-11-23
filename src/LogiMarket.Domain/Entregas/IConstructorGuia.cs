namespace LogiMarket.Domain.Entregas;

public interface IConstructorGuia
{
    TipoEntrega Familia { get; }
    string Construir(string numeroPedido, string cliente, DateOnly fechaDespacho);
}
