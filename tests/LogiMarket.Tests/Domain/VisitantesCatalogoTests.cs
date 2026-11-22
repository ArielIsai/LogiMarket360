using LogiMarket.Domain.Catalogo;
using Xunit;

namespace LogiMarket.Tests.Domain;

// P34: Visitor sobre KIT_TELETRABAJO (detalle P01/P02/P03 y peso 1.00 kg).
public class VisitantesCatalogoTests
{
    [Fact]
    public void El_desglose_enumera_los_SKU_finales_y_los_kits_atravesados()
    {
        var visitante = new VisitanteDesgloseFisico();
        CatalogoControl.KitTeletrabajo().Aceptar(visitante);

        var lineas = string.Join(",", visitante.Lineas.Select(l => $"{l.Producto.Sku}x{l.Cantidad}"));
        Assert.Equal("P01x1,P02x1,P03x1", lineas);
        Assert.Equal("KIT_TELETRABAJO,KIT_OFICINA", string.Join(",", visitante.KitsVisitados));   // visita anidada
    }

    [Fact]
    public void El_peso_logistico_de_KIT_TELETRABAJO_es_1_kg()
    {
        var visitante = new VisitantePesoLogistico();
        CatalogoControl.KitTeletrabajo().Aceptar(visitante);

        Assert.Equal(1.00m, visitante.PesoTotalKg);   // 0.20 + 0.70 + 0.10
    }

    [Fact]
    public void El_peso_escala_con_la_cantidad()
    {
        var visitante = new VisitantePesoLogistico();
        CatalogoControl.KitTeletrabajo().Aceptar(visitante, 2);

        Assert.Equal(2.00m, visitante.PesoTotalKg);
    }

    [Fact]
    public void Un_producto_suelto_tambien_acepta_visitantes()
    {
        var visitante = new VisitantePesoLogistico();
        CatalogoControl.P02().Aceptar(visitante, 3);

        Assert.Equal(2.10m, visitante.PesoTotalKg);   // 0.70 x 3
    }
}
