using System.Globalization;

namespace LogiMarket.Domain.Entregas;

/// <summary>Familia ESTÁNDAR: USD 5.00 y 3 días.</summary>
public sealed class FabricaEntregaEstandar : IFabricaEntrega
{
    public TipoEntrega Tipo => TipoEntrega.Estandar;

    public ICalculadorTarifa CrearCalculadorTarifa() => new TarifaEstandar();

    public ICalculadorPlazo CrearCalculadorPlazo() => new PlazoEstandar();

    // La guía se arma con el plazo de SU familia: no puede mostrar un plazo ajeno.
    public IConstructorGuia CrearConstructorGuia() => new GuiaEstandar(new PlazoEstandar());
}

// Productos internos: fuera del dominio solo se obtienen desde la fábrica, así no se mezclan familias a mano.
internal sealed class TarifaEstandar : ICalculadorTarifa
{
    public TipoEntrega Familia => TipoEntrega.Estandar;
    public decimal Calcular() => 5.00m;
}

internal sealed class PlazoEstandar : ICalculadorPlazo
{
    public TipoEntrega Familia => TipoEntrega.Estandar;
    public int DiasEstimados => 3;
    public DateOnly FechaEstimada(DateOnly desde) => desde.AddDays(DiasEstimados);
}

internal sealed class GuiaEstandar : IConstructorGuia
{
    private readonly ICalculadorPlazo _plazo;

    public GuiaEstandar(ICalculadorPlazo plazo) => _plazo = plazo;

    public TipoEntrega Familia => TipoEntrega.Estandar;

    public string Construir(string numeroPedido, string cliente, DateOnly fechaDespacho)
    {
        string fecha = _plazo.FechaEstimada(fechaDespacho).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
        return $"GUIA ESTANDAR | Pedido {numeroPedido} | Cliente {cliente} | Entrega estimada {fecha} ({_plazo.DiasEstimados} dias)";
    }
}
