namespace LogiMarket.Domain.Catalogo;

/// <summary>Visitor: desglose físico por SKU y registro de los kits atravesados.</summary>
public sealed class VisitanteDesgloseFisico : IVisitanteCatalogo
{
    private readonly List<ComponenteFisico> _lineas = new();
    private readonly List<string> _kits = new();

    public IReadOnlyList<ComponenteFisico> Lineas => _lineas;
    public IReadOnlyList<string> KitsVisitados => _kits;

    public void Visitar(ProductoSimple producto, int cantidad) =>
        _lineas.Add(new ComponenteFisico(producto, cantidad));

    public void Visitar(KitCompuesto kit, int cantidad) => _kits.Add(kit.Sku);
}
