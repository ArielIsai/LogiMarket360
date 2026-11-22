using LogiMarket.Domain.Comun;

namespace LogiMarket.Domain.Catalogo;

/// <summary>Hoja del Composite: un SKU físico con existencias.</summary>
public sealed class ProductoSimple : IElementoCatalogo
{
    public ProductoSimple(string sku, string nombre, decimal precio, decimal pesoKg, int stock)
    {
        if (string.IsNullOrWhiteSpace(sku)) throw new ExcepcionDominio("El SKU es obligatorio.");
        if (precio <= 0) throw new ExcepcionDominio("El precio debe ser mayor que cero.");
        if (pesoKg < 0) throw new ExcepcionDominio("El peso no puede ser negativo.");
        if (stock < 0) throw new ExcepcionDominio("El stock no puede ser negativo.");

        Sku = sku.Trim().ToUpperInvariant();   // SKU normalizado: la unicidad no depende de mayúsculas
        Nombre = nombre?.Trim() ?? "";
        Precio = precio;
        PesoKg = pesoKg;
        Stock = stock;
    }

    public string Sku { get; }
    public string Nombre { get; }
    public decimal Precio { get; }
    public decimal PesoKg { get; }
    public int Stock { get; }

    public bool Contiene(IElementoCatalogo otro)
    {
        ArgumentNullException.ThrowIfNull(otro);
        return otro.Sku == Sku;
    }

    public IteradorComponentesFisicos CrearIterador(int cantidad = 1) => new(this, cantidad);

    public void Aceptar(IVisitanteCatalogo visitante, int cantidad = 1) => visitante.Visitar(this, cantidad);
}
