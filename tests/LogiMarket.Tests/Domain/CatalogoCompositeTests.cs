using LogiMarket.Domain.Catalogo;
using LogiMarket.Domain.Comun;
using Xunit;

namespace LogiMarket.Tests.Domain;

// P18: precios 50/60, expansión anidada y ciclo rechazado.
public class CatalogoCompositeTests
{
    [Fact]
    public void KIT_OFICINA_cuesta_50()
    {
        Assert.Equal(50.00m, CatalogoControl.KitOficina().Precio);
    }

    [Fact]
    public void KIT_TELETRABAJO_anidado_cuesta_60()
    {
        Assert.Equal(60.00m, CatalogoControl.KitTeletrabajo().Precio);
    }

    [Fact]
    public void El_precio_del_kit_multiplica_por_la_cantidad_del_componente()
    {
        var kit = new KitCompuesto("KIT_X", "X");
        kit.Agregar(CatalogoControl.P01(), 3);
        Assert.Equal(60.00m, kit.Precio);
    }

    [Fact]
    public void Un_kit_no_puede_contenerse_a_si_mismo()
    {
        var kit = new KitCompuesto("KIT_A", "A");
        kit.Agregar(CatalogoControl.P01());

        Assert.Throws<ReferenciaCircularException>(() => kit.Agregar(kit));
        Assert.Equal(20.00m, kit.Precio);   // el rechazo no modificó el kit
    }

    [Fact]
    public void Un_ciclo_indirecto_se_rechaza_sin_modificar_el_kit()
    {
        var a = new KitCompuesto("KIT_A", "A");
        var b = new KitCompuesto("KIT_B", "B");
        a.Agregar(CatalogoControl.P01());
        b.Agregar(a);   // B contiene A

        var ex = Assert.Throws<ReferenciaCircularException>(() => a.Agregar(b));   // A contendría a B
        Assert.Equal("KIT_A", ex.Kit);
        Assert.Equal("KIT_B", ex.Componente);
        Assert.Equal(20.00m, a.Precio);
    }

    [Fact]
    public void Un_ciclo_de_tres_niveles_tambien_se_rechaza()
    {
        var a = new KitCompuesto("KIT_A", "A");
        var b = new KitCompuesto("KIT_B", "B");
        var c = new KitCompuesto("KIT_C", "C");
        a.Agregar(CatalogoControl.P01());
        b.Agregar(a);
        c.Agregar(b);

        Assert.Throws<ReferenciaCircularException>(() => a.Agregar(c));
    }

    [Fact]
    public void La_cantidad_de_un_componente_debe_ser_positiva()
    {
        var kit = new KitCompuesto("KIT_A", "A");
        Assert.Throws<ExcepcionDominio>(() => kit.Agregar(CatalogoControl.P01(), 0));
        Assert.Throws<ExcepcionDominio>(() => kit.Agregar(CatalogoControl.P01(), -1));
    }

    [Fact]
    public void El_producto_valida_sus_invariantes()
    {
        Assert.Throws<ExcepcionDominio>(() => new ProductoSimple("P01", "x", 0m, 0.1m, 1));       // precio no positivo
        Assert.Throws<ExcepcionDominio>(() => new ProductoSimple("P01", "x", -5m, 0.1m, 1));
        Assert.Throws<ExcepcionDominio>(() => new ProductoSimple("P01", "x", 10m, -0.01m, 1));    // peso negativo
        Assert.Throws<ExcepcionDominio>(() => new ProductoSimple("P01", "x", 10m, 0.1m, -1));     // stock negativo
        Assert.Throws<ExcepcionDominio>(() => new ProductoSimple("  ", "x", 10m, 0.1m, 1));       // SKU vacío
    }

    [Fact]
    public void El_peso_cero_y_el_stock_cero_son_validos()
    {
        var producto = new ProductoSimple("P09", "Sin peso", 1.00m, 0m, 0);
        Assert.Equal(0, producto.Stock);
    }

    [Fact]
    public void El_SKU_se_normaliza_a_mayusculas_sin_espacios()
    {
        var producto = new ProductoSimple("  p01 ", "Mouse", 20m, 0.2m, 8);
        Assert.Equal("P01", producto.Sku);
    }
}
