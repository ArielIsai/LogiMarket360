using LogiMarket.Domain.Catalogo;

namespace LogiMarket.Tests.Domain;

/// <summary>Catálogo de control de la guía (SKU, precios y pesos oficiales) para las pruebas de dominio.</summary>
internal static class CatalogoControl
{
    public static ProductoSimple P01() => new("P01", "Mouse USB", 20.00m, 0.20m, 8);
    public static ProductoSimple P02() => new("P02", "Teclado", 30.00m, 0.70m, 5);
    public static ProductoSimple P03() => new("P03", "Memoria USB", 10.00m, 0.10m, 20);

    public static KitCompuesto KitOficina()
    {
        var kit = new KitCompuesto("KIT_OFICINA", "Kit oficina");
        kit.Agregar(P01());
        kit.Agregar(P02());
        return kit;
    }

    public static KitCompuesto KitTeletrabajo()
    {
        var kit = new KitCompuesto("KIT_TELETRABAJO", "Kit teletrabajo");
        kit.Agregar(KitOficina());
        kit.Agregar(P03());
        return kit;
    }
}
