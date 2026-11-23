using LogiMarket.Domain.Comun;

namespace LogiMarket.Domain.Promociones;

/// <summary>
/// Elige UNA política: la aplicable de mayor prioridad. No hay cadena de condiciones:
/// agregar una política nueva es registrarla, sin editar esta clase.
/// </summary>
public sealed class SelectorPromocion
{
    private readonly IReadOnlyList<IPoliticaDescuento> _politicas;

    public SelectorPromocion(IEnumerable<IPoliticaDescuento> politicas)
    {
        ArgumentNullException.ThrowIfNull(politicas);
        var lista = politicas.ToList();

        if (lista.Count == 0)
            throw new ExcepcionDominio("Se requiere al menos una política de descuento.");

        // Con prioridades repetidas el resultado dependería del orden de registro: se rechaza.
        if (lista.GroupBy(p => p.Prioridad).Any(g => g.Count() > 1))
            throw new ExcepcionDominio("Dos políticas de descuento no pueden compartir prioridad.");

        _politicas = lista.OrderByDescending(p => p.Prioridad).ToList();
    }

    public IPoliticaDescuento Seleccionar(ContextoPromocion contexto) =>
        _politicas.FirstOrDefault(p => p.EsAplicable(contexto))
        ?? throw new ExcepcionDominio("Ninguna política es aplicable: falta la política sin descuento.");
}
