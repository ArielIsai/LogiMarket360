using System.Globalization;

namespace LogiMarket.Domain.Entregas;

/// <summary>Familia EXPRÉS: USD 12.00 y 1 día.</summary>
public sealed class FabricaEntregaExpres : IFabricaEntrega
{
    public TipoEntrega Tipo => TipoEntrega.Expres;

    public ICalculadorTarifa CrearCalculadorTarifa() => new TarifaExpres();

    public ICalculadorPlazo CrearCalculadorPlazo() => new PlazoExpres();

    public IConstructorGuia CrearConstructorGuia() => new GuiaExpres(new PlazoExpres());
}

internal sealed class TarifaExpres : ICalculadorTarifa
{
    public TipoEntrega Familia => TipoEntrega.Expres;
    public decimal Calcular() => 12.00m;
}

internal sealed class PlazoExpres : ICalculadorPlazo
{
    public TipoEntrega Familia => TipoEntrega.Expres;
    public int DiasEstimados => 1;
    public DateOnly FechaEstimada(DateOnly desde) => desde.AddDays(DiasEstimados);
}

internal sealed class GuiaExpres : IConstructorGuia
{
    private readonly ICalculadorPlazo _plazo;

    public GuiaExpres(ICalculadorPlazo plazo) => _plazo = plazo;

    public TipoEntrega Familia => TipoEntrega.Expres;

    public string Construir(string numeroPedido, string cliente, DateOnly fechaDespacho)
    {
        string fecha = _plazo.FechaEstimada(fechaDespacho).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        return $"GUIA EXPRES *PRIORIDAD* | Pedido {numeroPedido} | Cliente {cliente} | Entrega estimada {fecha} ({_plazo.DiasEstimados} dia)";
    }
}
