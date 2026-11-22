using LogiMarket.Domain.Comun;

namespace LogiMarket.Domain.Catalogo;

/// <summary>Componente de un kit con su cantidad. Interno: nadie fuera del dominio recibe la lista.</summary>
internal sealed record ComponenteKit(IElementoCatalogo Elemento, int Cantidad);

/// <summary>Nodo compuesto: contiene productos u otros kits e impide ciclos al agregar.</summary>
public sealed class KitCompuesto : IElementoCatalogo
{
    private readonly List<ComponenteKit> _componentes = new();

    public KitCompuesto(string sku, string nombre)
    {
        if (string.IsNullOrWhiteSpace(sku)) throw new ExcepcionDominio("El SKU es obligatorio.");
        Sku = sku.Trim().ToUpperInvariant();
        Nombre = nombre?.Trim() ?? "";
    }

    public string Sku { get; }
    public string Nombre { get; }

    /// <summary>Suma recursiva de precio × cantidad de cada componente, a 2 decimales.</summary>
    public decimal Precio => Dinero.Redondear(_componentes.Sum(c => c.Elemento.Precio * c.Cantidad));

    // Solo el iterador del propio dominio lee esta lista; no forma parte de la API pública.
    internal IReadOnlyList<ComponenteKit> ComponentesDirectos => _componentes;

    public void Agregar(IElementoCatalogo elemento, int cantidad = 1)
    {
        ArgumentNullException.ThrowIfNull(elemento);
        if (cantidad <= 0) throw new ExcepcionDominio("La cantidad de un componente debe ser un entero positivo.");

        // Se valida antes de modificar: un rechazo no deja el kit a medias.
        if (elemento.Contiene(this)) throw new ReferenciaCircularException(Sku, elemento.Sku);

        _componentes.Add(new ComponenteKit(elemento, cantidad));
    }

    public bool Contiene(IElementoCatalogo otro)
    {
        ArgumentNullException.ThrowIfNull(otro);
        return otro.Sku == Sku || _componentes.Any(c => c.Elemento.Contiene(otro));
    }

    public IteradorComponentesFisicos CrearIterador(int cantidad = 1) => new(this, cantidad);

    public void Aceptar(IVisitanteCatalogo visitante, int cantidad = 1)
    {
        visitante.Visitar(this, cantidad);
        foreach (var c in _componentes) c.Elemento.Aceptar(visitante, cantidad * c.Cantidad);
    }
}
