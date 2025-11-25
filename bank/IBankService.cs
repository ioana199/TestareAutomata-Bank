namespace bank
{
    public interface IBankService
    {
        string GetTransactionInfo(float amount, ICurrencyConverter converter);
    }

    public class BankService : IBankService
    {
        // Metoda 1: Folosirea Mock-ului (Simularea ratei de conversie)
        public string GetTransactionInfo(float amount, ICurrencyConverter converter)
        {
            float convertedAmount = converter.Convert(amount, "USD", "EUR");
            return $"Original Amount: {amount} USD. Converted Amount: {convertedAmount} EUR.";
        }
        
        // Metoda 2: O altă metodă care ar putea fi testată
        public bool IsWithdrawalAllowed(Account account, float amount)
        {
            return account.Balance - amount >= account.MinBalance;
        }
        
        // Metoda 3: O metodă care ar putea fi verificată dacă a fost apelată
        public void LogConversionRate(ICurrencyConverter converter)
        {
            // O metodă care doar apelează un serviciu extern de logging, simulăm asta
            // cu un apel la Convert, dar scopul ar fi altul
            converter.Convert(1, "EUR", "RON"); 
        }
    }
}
