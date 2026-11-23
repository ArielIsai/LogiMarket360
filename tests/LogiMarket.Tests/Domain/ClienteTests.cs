using LogiMarket.Domain.Clientes;
using LogiMarket.Domain.Comun;
using Xunit;

namespace LogiMarket.Tests.Domain;

public class ClienteTests
{
    [Fact]
    public void Un_cliente_valido_normaliza_los_espacios_y_nace_activo()
    {
        var ana = new Cliente(" 0102030405 ", "  Ana  ", CategoriaCliente.Vip);

        Assert.Equal("0102030405", ana.Identificacion);
        Assert.Equal("Ana", ana.Nombre);
        Assert.Equal(CategoriaCliente.Vip, ana.Categoria);
        Assert.True(ana.Activo);
    }

    [Fact]
    public void La_identificacion_y_el_nombre_son_obligatorios()
    {
        Assert.Throws<ExcepcionDominio>(() => new Cliente("  ", "Ana"));
        Assert.Throws<ExcepcionDominio>(() => new Cliente("0102030405", ""));
    }

    [Fact]
    public void Se_puede_desactivar_reactivar_y_cambiar_de_categoria()
    {
        var luis = new Cliente("0911111111", "Luis");

        luis.Desactivar();
        Assert.False(luis.Activo);
        luis.Activar();
        Assert.True(luis.Activo);

        luis.CambiarCategoria(CategoriaCliente.Vip);
        Assert.Equal(CategoriaCliente.Vip, luis.Categoria);
    }

    [Fact]
    public void Una_categoria_inexistente_se_rechaza_y_conserva_la_anterior()
    {
        var luis = new Cliente("0911111111", "Luis");

        Assert.Throws<ExcepcionDominio>(() => luis.CambiarCategoria((CategoriaCliente)99));
        Assert.Equal(CategoriaCliente.Normal, luis.Categoria);
    }

    [Fact]
    public void El_cliente_no_expone_setters_publicos()
    {
        var conSetterPublico = typeof(Cliente).GetProperties()
            .Where(p => p.SetMethod is { IsPublic: true })
            .Select(p => p.Name)
            .ToList();

        Assert.Empty(conSetterPublico);
    }
}
