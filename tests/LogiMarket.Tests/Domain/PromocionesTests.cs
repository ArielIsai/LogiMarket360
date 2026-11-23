using LogiMarket.Domain.Clientes;
using LogiMarket.Domain.Comun;
using LogiMarket.Domain.Promociones;
using Xunit;

namespace LogiMarket.Tests.Domain;

// Strategy: Ana descuento 6.00; Carmen 15.00; sin descuento 0.00; nunca se acumulan.
public class PromocionesTests
{
    // El orden de registro no importa: la prioridad es explícita.
    private static SelectorPromocion Selector() =>
        new(new IPoliticaDescuento[] { new Mayorista15(), new SinDescuento(), new Vip10() });

    private static string Evaluar(CategoriaCliente categoria, int unidades, decimal subtotal)
    {
        var politica = Selector().Seleccionar(new ContextoPromocion(categoria, unidades));
        return $"{politica.Nombre}:{Dinero.Texto(politica.CalcularDescuento(subtotal))}";
    }

    [Fact]
    public void Ana_VIP_con_un_subtotal_de_60_recibe_6()
    {
        Assert.Equal("VIP:6.00", Evaluar(CategoriaCliente.Vip, 3, 60.00m));
    }

    [Fact]
    public void Carmen_con_10_unidades_recibe_15_sobre_100()
    {
        Assert.Equal("MAYORISTA:15.00", Evaluar(CategoriaCliente.Normal, 10, 100.00m));
    }

    [Fact]
    public void Un_cliente_normal_con_pocas_unidades_no_recibe_descuento()
    {
        Assert.Equal("NORMAL:0.00", Evaluar(CategoriaCliente.Normal, 4, 90.00m));
    }

    [Fact]
    public void Si_coinciden_VIP_y_MAYORISTA_gana_MAYORISTA_y_no_se_acumulan()
    {
        Assert.Equal("MAYORISTA:15.00", Evaluar(CategoriaCliente.Vip, 10, 100.00m));   // no 25.00
    }

    [Fact]
    public void El_umbral_mayorista_es_exactamente_10_unidades()
    {
        Assert.Equal("NORMAL:0.00", Evaluar(CategoriaCliente.Normal, 9, 100.00m));
        Assert.Equal("MAYORISTA:15.00", Evaluar(CategoriaCliente.Normal, 10, 100.00m));
    }

    [Fact]
    public void El_descuento_se_redondea_AwayFromZero()
    {
        Assert.Equal("VIP:0.13", Evaluar(CategoriaCliente.Vip, 1, 1.25m));   // 0.125 -> 0.13 (bancario daría 0.12)
    }

    // Política creada solo para esta prueba: demuestra OCP, el selector no se toca.
    private sealed class VipPlus : PoliticaPorTasa
    {
        public override string Nombre => "VIP_PLUS";
        public override int Prioridad => 15;
        public override decimal Tasa => 0.20m;
        public override bool EsAplicable(ContextoPromocion c) => c.Categoria == CategoriaCliente.Vip && c.UnidadesFisicas >= 5;
    }

    [Fact]
    public void Una_politica_nueva_se_agrega_sin_editar_el_selector()
    {
        var selector = new SelectorPromocion(new IPoliticaDescuento[]
            { new SinDescuento(), new Vip10(), new Mayorista15(), new VipPlus() });

        var vipConCinco = selector.Seleccionar(new ContextoPromocion(CategoriaCliente.Vip, 5));
        var vipConTres = selector.Seleccionar(new ContextoPromocion(CategoriaCliente.Vip, 3));
        var vipConDiez = selector.Seleccionar(new ContextoPromocion(CategoriaCliente.Vip, 10));

        Assert.Equal("VIP_PLUS", vipConCinco.Nombre);
        Assert.Equal("VIP", vipConTres.Nombre);
        Assert.Equal("MAYORISTA", vipConDiez.Nombre);   // la prioridad 20 sigue ganando
    }

    [Fact]
    public void El_selector_rechaza_prioridades_repetidas_y_listas_vacias()
    {
        Assert.Throws<ExcepcionDominio>(() => new SelectorPromocion(new IPoliticaDescuento[] { new Vip10(), new Vip10() }));
        Assert.Throws<ExcepcionDominio>(() => new SelectorPromocion(new List<IPoliticaDescuento>()));
    }

    [Fact]
    public void Sin_politica_de_respaldo_un_cliente_sin_promocion_es_un_error()
    {
        var selector = new SelectorPromocion(new IPoliticaDescuento[] { new Vip10() });

        Assert.Throws<ExcepcionDominio>(() => selector.Seleccionar(new ContextoPromocion(CategoriaCliente.Normal, 1)));
    }
}
