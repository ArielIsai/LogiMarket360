using LogiMarket.Domain.Clientes;
using LogiMarket.Domain.Comun;

namespace LogiMarket.Domain.Promociones;

/// <summary>Datos que deciden la promoción: categoría del cliente y unidades físicas finales del pedido.</summary>
public readonly record struct ContextoPromocion(CategoriaCliente Categoria, int UnidadesFisicas);

/// <summary>Strategy: cada política decide si aplica y cuánto descuenta; quien calcula solo conoce esta interfaz.</summary>
public interface IPoliticaDescuento
{
    string Nombre { get; }

    /// <summary>Prioridad explícita: gana la aplicable con el número más alto. Los descuentos no se acumulan.</summary>
    int Prioridad { get; }

    bool EsAplicable(ContextoPromocion contexto);

    decimal CalcularDescuento(decimal subtotal);
}

/// <summary>Base de las políticas porcentuales: la fórmula del descuento vive en un solo lugar.</summary>
public abstract class PoliticaPorTasa : IPoliticaDescuento
{
    public abstract string Nombre { get; }
    public abstract int Prioridad { get; }
    public abstract decimal Tasa { get; }

    public abstract bool EsAplicable(ContextoPromocion contexto);

    public decimal CalcularDescuento(decimal subtotal) => Dinero.AplicarTasa(subtotal, Tasa);
}
