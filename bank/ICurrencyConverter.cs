namespace bank
{
    public interface ICurrencyConverter
    {
        float Convert(float amount, string fromCurrency, string toCurrency);
    }
}
