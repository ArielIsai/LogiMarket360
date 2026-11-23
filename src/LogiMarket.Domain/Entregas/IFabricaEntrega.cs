namespace LogiMarket.Domain.Entregas;

public enum TipoEntrega
{
    Estandar = 0,
    Expres = 1
}

/// <summary>
/// Abstract Factory: cada fábrica entrega la familia completa de una forma de entrega
/// (tarifa, plazo y guía), de modo que los tres objetos son siempre compatibles entre sí.
/// </summary>
public interface IFabricaEntrega
{
    TipoEntrega Tipo { get; }
    ICalculadorTarifa CrearCalculadorTarifa();
    ICalculadorPlazo CrearCalculadorPlazo();
    IConstructorGuia CrearConstructorGuia();
}
