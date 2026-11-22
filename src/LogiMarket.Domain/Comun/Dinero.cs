using System.Globalization;

namespace LogiMarket.Domain.Comun;

/// <summary>
/// Reglas monetarias de la práctica: decimal, redondeo MidpointRounding.AwayFromZero
/// a 2 decimales y texto con punto decimal. Sin IVA (la guía lo excluye).
/// </summary>
public static class Dinero
{
    public const int Decimales = 2;

    public static decimal Redondear(decimal valor) =>
        Math.Round(valor, Decimales, MidpointRounding.AwayFromZero);

    /// <summary>descuento = redondear(subtotal × tasa).</summary>
    public static decimal AplicarTasa(decimal subtotal, decimal tasa) =>
        Redondear(subtotal * tasa);

    public static string Texto(decimal valor) =>
        Redondear(valor).ToString("0.00", CultureInfo.InvariantCulture);
}
