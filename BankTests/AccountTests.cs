using NUnit.Framework;
using bank; // Asigură-te că namespace-ul corespunde

namespace BankTests
{
    [TestFixture]
    public class AccountTests
    {
        // 1. Test pentru constructorul implicit
        [Test]
        public void Account_DefaultConstructor_InitialBalanceIsZero()
        {
            var account = new Account();
            Assert.That(account.Balance, Is.EqualTo(0));
        }

        // 2. Test pentru constructorul cu valoare
        [Test]
        public void Account_ConstructorWithValue_SetsCorrectBalance()
        {
            float initialValue = 100.50f;
            var account = new Account(initialValue);
            Assert.That(account.Balance, Is.EqualTo(initialValue));
        }

        // 3. Test pentru depunere (cazul de succes)
        [Test]
        public void Deposit_PositiveAmount_IncreasesBalance()
        {
            var account = new Account(50);
            account.Deposit(25.50f);
            Assert.That(account.Balance, Is.EqualTo(75.50f));
        }

        // 4. Test pentru depunere (cazul negativ - aruncă excepție)
        [Test]
        public void Deposit_NegativeAmount_ThrowsArgumentException()
        {
            var account = new Account(50);
            Assert.Throws<ArgumentException>(() => account.Deposit(-10f));
        }

        // 5. Test pentru retragere (cazul de succes)
        [Test]
        public void Withdraw_ValidAmount_DecreasesBalance()
        {
            var account = new Account(100);
            account.Withdraw(30f);
            Assert.That(account.Balance, Is.EqualTo(70f));
        }

        // 6. Test pentru retragere (fonduri insuficiente)
        [Test]
        public void Withdraw_ExceedingBalance_ThrowsNotEnoughFundsException()
        {
            var account = new Account(20);
            Assert.Throws<NotEnoughFundsException>(() => account.Withdraw(30f));
        }

        // 7. Test pentru TransferMinFunds (transfer reușit, respectă soldul minim)
        [Test]
        public void TransferMinFunds_SufficientFunds_TransferSucceeds()
        {
            float initialSourceBalance = 100f;
            float transferAmount = 80f; // Soldul final: 100 - 80 = 20 ( > MinBalance 10)
            var sourceAccount = new Account(initialSourceBalance);
            var destinationAccount = new Account(0);

            sourceAccount.TransferMinFunds(destinationAccount, transferAmount);

            Assert.That(sourceAccount.Balance, Is.EqualTo(20f));
            Assert.That(destinationAccount.Balance, Is.EqualTo(transferAmount));
        }

        // 8. Test pentru TransferMinFunds (transfer care încalcă soldul minim)
        [Test]
        public void TransferMinFunds_ViolatesMinBalance_ThrowsNotEnoughFundsException()
        {
            float initialSourceBalance = 50f;
            float transferAmount = 45f; // Soldul final: 50 - 45 = 5 ( < MinBalance 10)
            var sourceAccount = new Account(initialSourceBalance);
            var destinationAccount = new Account(0);

            Assert.Throws<NotEnoughFundsException>(() => 
                sourceAccount.TransferMinFunds(destinationAccount, transferAmount));
        }

        // 9. Test pentru TransferFunds (transfer reușit)
        [Test]
        public void TransferFunds_Simple_Succeeds()
        {
            var source = new Account(100);
            var destination = new Account(50);
            source.TransferFunds(destination, 20f);
            
            Assert.That(source.Balance, Is.EqualTo(80f));
            Assert.That(destination.Balance, Is.EqualTo(70f));
        }
        
        // 10. Test de margine: TransferMinFunds exact la limita MinBalance
        [Test]
        public void TransferMinFunds_LeavesExactlyMinBalance_TransferSucceeds()
        {
            float min = new Account().MinBalance;
            float initial = 100f;
            float transfer = initial - min; // 90.0f
            var source = new Account(initial);
            var destination = new Account(0);

            source.TransferMinFunds(destination, transfer);
            
            Assert.That(source.Balance, Is.EqualTo(min));
            Assert.That(destination.Balance, Is.EqualTo(transfer));
        }
    }
}
