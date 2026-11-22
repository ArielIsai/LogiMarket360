namespace LogiMarket.Domain.Catalogo;

/// <summary>Visitor: estima el peso logístico total; el peso de un kit es la suma de sus SKU finales.</summary>
public sealed class VisitantePesoLogistico : IVisitanteCatalogo
{
    public decimal PesoTotalKg { get; private set; }

    public void Visitar(ProductoSimple producto, int cantidad) => PesoTotalKg += producto.PesoKg * cantidad;

    public void Visitar(KitCompuesto kit, int cantidad)
    {
        // Un kit no pesa por sí mismo: su aporte llega al visitar sus productos finales.
    }
}
