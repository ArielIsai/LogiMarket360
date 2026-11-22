namespace LogiMarket.Domain.Catalogo;

/// <summary>
/// Visitor: permite agregar operaciones sobre el catálogo (desglose, peso...)
/// sin añadir esos reportes a ProductoSimple ni a KitCompuesto.
/// </summary>
public interface IVisitanteCatalogo
{
    /// <param name="cantidad">Unidades acumuladas desde la raíz visitada.</param>
    void Visitar(ProductoSimple producto, int cantidad);
    void Visitar(KitCompuesto kit, int cantidad);
}
