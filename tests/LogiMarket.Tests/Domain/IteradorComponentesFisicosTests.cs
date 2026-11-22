using System.Collections;
using LogiMarket.Domain.Catalogo;
using Xunit;

namespace LogiMarket.Tests.Domain;

// P27: recorrido determinista de KIT_TELETRABAJO, con cantidades y sin bucle.
public class IteradorComponentesFisicosTests
{
    private static string Recorrer(IElementoCatalogo elemento, int cantidad = 1)
    {
        var partes = new List<string>();
        using var iterador = elemento.CrearIterador(cantidad);
        while (iterador.MoveNext())
            partes.Add($"{iterador.Current.Producto.Sku}x{iterador.Current.Cantidad}");
        return string.Join(",", partes);
    }

    [Fact]
    public void KIT_TELETRABAJO_devuelve_P01_P02_P03_con_sus_cantidades()
    {
        Assert.Equal("P01x1,P02x1,P03x1", Recorrer(CatalogoControl.KitTeletrabajo()));
    }

    [Fact]
    public void Las_cantidades_se_propagan_desde_la_raiz()
    {
        Assert.Equal("P01x6,P02x6,P03x6", Recorrer(CatalogoControl.KitTeletrabajo(), 6));
    }

    [Fact]
    public void Las_cantidades_se_multiplican_por_nivel()
    {
        var kit = new KitCompuesto("KIT_X", "X");
        kit.Agregar(CatalogoControl.KitOficina(), 2);   // dos kits de oficina
        kit.Agregar(CatalogoControl.P03(), 3);
        Assert.Equal("P01x2,P02x2,P03x3", Recorrer(kit));
    }

    [Fact]
    public void Un_producto_suelto_se_recorre_como_un_solo_elemento()
    {
        Assert.Equal("P02x4", Recorrer(CatalogoControl.P02(), 4));
    }

    [Fact]
    public void El_iterador_termina_y_sigue_devolviendo_false()
    {
        using var iterador = CatalogoControl.KitTeletrabajo().CrearIterador();
        int vueltas = 0;
        while (iterador.MoveNext()) vueltas++;

        Assert.Equal(3, vueltas);
        Assert.False(iterador.MoveNext());
        Assert.False(iterador.MoveNext());
    }

    [Fact]
    public void Reset_permite_recorrer_de_nuevo_con_el_mismo_resultado()
    {
        using var iterador = CatalogoControl.KitTeletrabajo().CrearIterador();
        while (iterador.MoveNext()) { }
        iterador.Reset();

        Assert.True(iterador.MoveNext());
        Assert.Equal("P01", iterador.Current.Producto.Sku);
    }

    [Fact]
    public void Leer_Current_antes_de_MoveNext_es_un_error()
    {
        using var iterador = CatalogoControl.KitTeletrabajo().CrearIterador();
        Assert.Throws<InvalidOperationException>(() => { _ = iterador.Current; });
    }

    [Fact]
    public void SkusFinales_consolida_un_mismo_SKU_que_llega_por_dos_caminos()
    {
        var kit = new KitCompuesto("KIT_X", "X");
        kit.Agregar(CatalogoControl.KitOficina(), 2);   // P01 x2, P02 x2
        kit.Agregar(CatalogoControl.P01(), 1);          // P01 x1 por otro camino

        var resumen = string.Join(",", kit.SkusFinales().Select(c => $"{c.Producto.Sku}x{c.Cantidad}"));
        Assert.Equal("P01x3,P02x2", resumen);
    }

    [Fact]
    public void El_kit_no_expone_su_coleccion_interna()
    {
        var colecciones = typeof(KitCompuesto).GetProperties()
            .Where(p => typeof(IEnumerable).IsAssignableFrom(p.PropertyType) && p.PropertyType != typeof(string))
            .Select(p => p.Name)
            .ToList();

        Assert.Empty(colecciones);
    }
}
