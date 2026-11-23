namespace LogiMarket.Domain.Promociones;

/// <summary>MAYORISTA: 15 % desde 10 unidades físicas finales, sin importar la categoría del cliente.</summary>
public sealed class Mayorista15 : PoliticaPorTasa
{
    public const int UnidadesMinimas = 10;

    public override string Nombre => "MAYORISTA";
    public override int Prioridad => 20;   // mayor que VIP: si coinciden, gana MAYORISTA
    public override decimal Tasa => 0.15m;

    public override bool EsAplicable(ContextoPromocion contexto) => contexto.UnidadesFisicas >= UnidadesMinimas;
}
