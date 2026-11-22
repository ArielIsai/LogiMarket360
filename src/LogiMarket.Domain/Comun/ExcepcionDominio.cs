namespace LogiMarket.Domain.Comun;

/// <summary>Violación de una invariante o regla del dominio; Application la traduce a un mensaje claro.</summary>
public class ExcepcionDominio : Exception
{
    public ExcepcionDominio(string mensaje) : base(mensaje) { }
}

/// <summary>Un kit no puede contenerse a sí mismo, ni directa ni indirectamente.</summary>
public sealed class ReferenciaCircularException : ExcepcionDominio
{
    public ReferenciaCircularException(string kit, string componente)
        : base($"El kit {kit} no puede contener a {componente}: se crearía una referencia circular.")
    {
        Kit = kit;
        Componente = componente;
    }

    public string Kit { get; }
    public string Componente { get; }
}
