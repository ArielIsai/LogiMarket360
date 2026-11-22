using LogiMarket.Domain.Comun;
using Xunit;

namespace LogiMarket.Tests.Domain;

public class DineroTests
{
    [Fact]
    public void Redondear_usa_AwayFromZero_donde_el_redondeo_bancario_difiere()
    {
        Assert.Equal(2.67m, Dinero.Redondear(2.665m));    // bancario daría 2.66
        Assert.Equal(0.13m, Dinero.Redondear(0.125m));    // bancario daría 0.12
        Assert.Equal(0.01m, Dinero.Redondear(0.005m));    // bancario daría 0.00
        Assert.Equal(-2.67m, Dinero.Redondear(-2.665m));
    }

    [Fact]
    public void Descuento_VIP_de_un_subtotal_de_60_es_6()
    {
        Assert.Equal(6.00m, Dinero.AplicarTasa(60.00m, 0.10m));
    }

    [Fact]
    public void Texto_muestra_siempre_dos_decimales_con_punto()
    {
        Assert.Equal("59.00", Dinero.Texto(59m));
        Assert.Equal("0.01", Dinero.Texto(0.005m));
    }
}
