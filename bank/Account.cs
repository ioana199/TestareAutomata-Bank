using System;

namespace bank
{
    // Aici ar fi clasa NotEnoughFundsException (dacă nu e în același fișier)
    public class NotEnoughFundsException : ApplicationException
    {
        public NotEnoughFundsException() : base("Insufficient funds to complete the operation.") { }
        public NotEnoughFundsException(string message) : base(message) { }
    }

    public class Account
    {
        private float balance;
        private readonly float minBalance = 10; // Folosim readonly pentru o constantă la nivel de instanță

        public Account()
        {
            balance = 0;
        }

        public Account(float initialValue) // Am schimbat int în float
        {
            if (initialValue < 0)
            {
                throw new ArgumentException("Initial balance cannot be negative.");
            }
            balance = initialValue;
        }

        public void Deposit(float amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Deposit amount must be positive.");
            }
            balance += amount;
        }

        public void Withdraw(float amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Withdrawal amount must be positive.");
            }

            // Presupunem că Withdraw clasic nu verifică MinBalance
            if (balance - amount < 0)
            {
                throw new NotEnoughFundsException("Withdrawal exceeds the current balance.");
            }
            
            balance -= amount;
        }

        // Metoda TransferFunds nu e ideală, o voi păstra dar adaug un test care să o pună în dificultate
        public void TransferFunds(Account destination, float amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Transfer amount must be positive.");
            }
            if (balance - amount < 0) // Verificare simplă de fonduri
            {
                throw new NotEnoughFundsException("Insufficient funds for TransferFunds operation.");
            }
            destination.Deposit(amount);
            Withdraw(amount); // Rețineți că Withdraw-ul de mai sus aruncă excepție, dar aici e verificat deja
        }

        public Account TransferMinFunds(Account destination, float amount)
        {
            if (amount <= 0)
            {
                throw new ArgumentException("Transfer amount must be positive.");
            }
            if (balance - amount >= MinBalance)
            {
                destination.Deposit(amount);
                // Nu mai apelăm Withdraw direct, modificăm balance local
                balance -= amount; 
            }
            else
            {
                throw new NotEnoughFundsException("Transfer would violate the minimum balance requirement.");
            }
            return destination;
        }

        public float Balance
        {
            get { return balance; }
        }
        public float MinBalance
        {
            get { return minBalance; }
        }
    }
}
