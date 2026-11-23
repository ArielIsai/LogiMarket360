using LogiMarket.Domain.Comun;
using LogiMarket.Domain.Entregas;
using Xunit;

namespace LogiMarket.Tests.Domain;

// Abstract Factory: ESTÁNDAR = 5.00 / 3 días y EXPRÉS = 12.00 / 1 día, con familias compatibles.
public class EntregasTests
{
    private static readonly DateOnly Despacho = new(2026, 11, 25);

    // El mismo código sirve para ambas familias: no sabe cuál recibe (sustituibilidad).
    private static string Resumen(IFabricaEntrega fabrica)
    {
        var plazo = fabrica.CrearCalculadorPlazo();
        return $"{Dinero.Texto(fabrica.CrearCalculadorTarifa().Calcular())}|{plazo.DiasEstimados}|{plazo.FechaEstimada(Despacho):yyyy-MM-dd}";
    }

    [Fact]
    public void ESTANDAR_cobra_5_y_tarda_3_dias()
    {
        Assert.Equal("5.00|3|2026-11-28", Resumen(new FabricaEntregaEstandar()));
    }

    [Fact]
    public void EXPRES_cobra_12_y_tarda_1_dia()
    {
        Assert.Equal("12.00|1|2026-11-26", Resumen(new FabricaEntregaExpres()));
    }

    [Fact]
    public void Cada_fabrica_entrega_una_familia_compatible()
    {
        foreach (IFabricaEntrega fabrica in new IFabricaEntrega[] { new FabricaEntregaEstandar(), new FabricaEntregaExpres() })
        {
            Assert.Equal(fabrica.Tipo, fabrica.CrearCalculadorTarifa().Familia);
            Assert.Equal(fabrica.Tipo, fabrica.CrearCalculadorPlazo().Familia);
            Assert.Equal(fabrica.Tipo, fabrica.CrearConstructorGuia().Familia);
        }
    }

    [Fact]
    public void Las_guias_distinguen_la_familia_y_usan_el_plazo_de_su_familia()
    {
        var estandar = new FabricaEntregaEstandar().CrearConstructorGuia().Construir("15", "Ana", Despacho);
        var expres = new FabricaEntregaExpres().CrearConstructorGuia().Construir("15", "Ana", Despacho);

        Assert.Equal("GUIA ESTANDAR | Pedido 15 | Cliente Ana | Entrega estimada 2026-11-28 (3 dias)", estandar);
        Assert.Equal("GUIA EXPRES *PRIORIDAD* | Pedido 15 | Cliente Ana | Entrega estimada 2026-11-26 (1 dia)", expres);
    }

    [Fact]
    public void Los_productos_concretos_no_son_publicos_y_solo_salen_de_la_fabrica()
    {
        var publicos = typeof(IFabricaEntrega).Assembly.GetTypes()
            .Where(t => t.IsPublic && t.IsClass)
            .Where(t => t.Name.StartsWith("Tarifa") || t.Name.StartsWith("Plazo") || t.Name.StartsWith("Guia"))
            .Select(t => t.Name)
            .ToList();

        Assert.Empty(publicos);
    }
}
