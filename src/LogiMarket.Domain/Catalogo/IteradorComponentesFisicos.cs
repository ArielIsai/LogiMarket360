using System.Collections;
using LogiMarket.Domain.Comun;

namespace LogiMarket.Domain.Catalogo;

/// <summary>Producto físico final con las unidades acumuladas desde la raíz recorrida.</summary>
public sealed record ComponenteFisico(ProductoSimple Producto, int Cantidad);

/// <summary>
/// Iterator propio: recorre en profundidad, con pila explícita, los productos finales de un
/// producto o kit anidado multiplicando cantidades. No entrega la colección interna del kit.
/// </summary>
public sealed class IteradorComponentesFisicos : IEnumerator<ComponenteFisico>
{
    private sealed class Marco
    {
        public Marco(IReadOnlyList<ComponenteKit> componentes, int multiplicador)
        {
            Componentes = componentes;
            Multiplicador = multiplicador;
        }

        public IReadOnlyList<ComponenteKit> Componentes { get; }
        public int Multiplicador { get; }
        public int Indice { get; set; }
    }

    private readonly IElementoCatalogo _raiz;
    private readonly int _cantidad;
    private readonly Stack<Marco> _pila = new();
    private ComponenteFisico? _actual;
    private bool _iniciado;

    internal IteradorComponentesFisicos(IElementoCatalogo raiz, int cantidad)
    {
        ArgumentNullException.ThrowIfNull(raiz);
        if (cantidad <= 0) throw new ExcepcionDominio("La cantidad a recorrer debe ser un entero positivo.");
        _raiz = raiz;
        _cantidad = cantidad;
    }

    public ComponenteFisico Current =>
        _actual ?? throw new InvalidOperationException("Llame a MoveNext() antes de leer Current.");

    object IEnumerator.Current => Current;

    public bool MoveNext()
    {
        if (!_iniciado)
        {
            Apilar(_raiz, _cantidad);
            _iniciado = true;
        }

        while (_pila.Count > 0)
        {
            var marco = _pila.Peek();
            if (marco.Indice >= marco.Componentes.Count)
            {
                _pila.Pop();
                continue;
            }

            var componente = marco.Componentes[marco.Indice++];
            int cantidad = marco.Multiplicador * componente.Cantidad;

            if (componente.Elemento is ProductoSimple producto)
            {
                _actual = new ComponenteFisico(producto, cantidad);
                return true;
            }

            Apilar(componente.Elemento, cantidad);   // kit anidado: se desciende sin recursión
        }

        _actual = null;
        return false;
    }

    private void Apilar(IElementoCatalogo elemento, int multiplicador)
    {
        IReadOnlyList<ComponenteKit> componentes = elemento is KitCompuesto kit
            ? kit.ComponentesDirectos
            : new[] { new ComponenteKit(elemento, 1) };   // un producto suelto se trata como kit de un componente
        _pila.Push(new Marco(componentes, multiplicador));
    }

    public void Reset()
    {
        _pila.Clear();
        _actual = null;
        _iniciado = false;
    }

    public void Dispose() { }
}

/// <summary>Consolida el recorrido por SKU: es la base del control de existencias.</summary>
public static class ExpansionCatalogo
{
    public static IReadOnlyList<ComponenteFisico> SkusFinales(this IElementoCatalogo elemento, int cantidad = 1)
    {
        var orden = new List<string>();
        var acumulado = new Dictionary<string, ComponenteFisico>();

        using var iterador = elemento.CrearIterador(cantidad);
        while (iterador.MoveNext())
        {
            var actual = iterador.Current;
            string sku = actual.Producto.Sku;
            if (acumulado.TryGetValue(sku, out var previo))
                acumulado[sku] = previo with { Cantidad = previo.Cantidad + actual.Cantidad };
            else
            {
                acumulado[sku] = actual;
                orden.Add(sku);
            }
        }

        return orden.Select(sku => acumulado[sku]).ToList();
    }
}
