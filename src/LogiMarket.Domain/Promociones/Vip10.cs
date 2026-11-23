using LogiMarket.Domain.Clientes;

namespace LogiMarket.Domain.Promociones;

/// <summary>VIP: 10 % para clientes de esa categoría, siempre que no corresponda MAYORISTA.</summary>
public sealed class Vip10 : PoliticaPorTasa
{
    public override string Nombre => "VIP";
    public override int Prioridad => 10;
    public override decimal Tasa => 0.10m;

    public override bool EsAplicable(ContextoPromocion contexto) => contexto.Categoria == CategoriaCliente.Vip;
}
