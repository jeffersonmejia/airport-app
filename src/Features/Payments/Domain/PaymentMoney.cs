using System.Globalization;

namespace Airport.Features.Payments.Domain;

public readonly record struct PaymentMoney
{
    private PaymentMoney(decimal amount, string currencyCode)
    {
        Amount = amount;
        CurrencyCode = currencyCode;
    }

    public decimal Amount { get; }

    public string CurrencyCode { get; }

    public static PaymentMoney Create(decimal amount, string currencyCode)
    {
        if (amount <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(amount), "El monto debe ser mayor que cero.");
        }

        var normalizedCurrency = currencyCode.Trim().ToUpperInvariant();
        if (normalizedCurrency != "USD")
        {
            throw new ArgumentException("La moneda permitida para este examen es USD.", nameof(currencyCode));
        }

        return new PaymentMoney(decimal.Round(amount, 2, MidpointRounding.AwayFromZero), normalizedCurrency);
    }

    public string ToPayPalValue() => Amount.ToString("0.00", CultureInfo.InvariantCulture);
}
