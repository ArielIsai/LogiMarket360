namespace LogiMarket.Domain.Catalogo;

/// <summary>Composite: un producto y un kit (que puede contener otros kits) se usan de la misma forma.</summary>
public interface IElementoCatalogo
{
    string Sku { get; }
    string Nombre { get; }

    /// <summary>Precio de una unidad. En un kit es la suma recursiva de sus componentes.</summary>
    decimal Precio { get; }

    /// <summary>true si este elemento es, o contiene en cualquier nivel, a otro con el mismo SKU.</summary>
    bool Contiene(IElementoCatalogo otro);

    /// <summary>Iterator propio sobre los productos físicos finales, con cantidades acumuladas.</summary>
    IteradorComponentesFisicos CrearIterador(int cantidad = 1);

    /// <summary>Entrada del Visitor; un kit propaga la visita a sus componentes.</summary>
    void Aceptar(IVisitanteCatalogo visitante, int cantidad = 1);
}
