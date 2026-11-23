namespace LogiMarket.Domain.Promociones;

/// <summary>Política de respaldo (NORMAL, 0 %): siempre aplica, con la prioridad más baja.</summary>
public sealed class SinDescuento : PoliticaPorTasa
{
    public override string Nombre => "NORMAL";
    public override int Prioridad => 0;
    public override decimal Tasa => 0m;

    public override bool EsAplicable(ContextoPromocion contexto) => true;
}
