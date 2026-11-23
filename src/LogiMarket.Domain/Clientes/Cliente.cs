using LogiMarket.Domain.Comun;

namespace LogiMarket.Domain.Clientes;

/// <summary>Cliente del comercio. Los cambios pasan por métodos, no por setters públicos.</summary>
public sealed class Cliente
{
    public Cliente(string identificacion, string nombre,
        CategoriaCliente categoria = CategoriaCliente.Normal, bool activo = true)
    {
        if (string.IsNullOrWhiteSpace(identificacion))
            throw new ExcepcionDominio("La identificación del cliente es obligatoria.");
        if (string.IsNullOrWhiteSpace(nombre))
            throw new ExcepcionDominio("El nombre del cliente es obligatorio.");
        ValidarCategoria(categoria);

        Identificacion = identificacion.Trim();
        Nombre = nombre.Trim();
        Categoria = categoria;
        Activo = activo;
    }

    public string Identificacion { get; }
    public string Nombre { get; }
    public CategoriaCliente Categoria { get; private set; }
    public bool Activo { get; private set; }

    public void CambiarCategoria(CategoriaCliente categoria)
    {
        ValidarCategoria(categoria);
        Categoria = categoria;
    }

    public void Activar() => Activo = true;

    public void Desactivar() => Activo = false;

    private static void ValidarCategoria(CategoriaCliente categoria)
    {
        if (!Enum.IsDefined(categoria))
            throw new ExcepcionDominio("La categoría del cliente no es válida.");
    }
}
