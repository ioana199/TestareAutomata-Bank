using NUnit.Framework;
using Moq; // Biblioteca Moq
using bank;
using System;

namespace BankTests
{
    [TestFixture]
    public class BankServiceTests
    {
        // Mock object pentru ICurrencyConverter
        private Mock<ICurrencyConverter> _mockConverter; 
        private BankService _bankService;

        [SetUp]
        public void Setup()
        {
            // Inițializarea Mock-ului și a serviciului înainte de fiecare test
            _mockConverter = new Mock<ICurrencyConverter>();
            _bankService = new BankService();
        }

        // Test 1: Simulează o valoare returnată (GetTransactionInfo)
        [Test]
        public void GetTransactionInfo_ShouldReturnCorrectConvertedValue()
        {
            float originalAmount = 100f;
            float expectedConvertedAmount = 120f;
            
            // **Setup Moq**: Simulăm că, indiferent de sumă, conversia de la USD la EUR 
            // returnează expectedConvertedAmount.
            // O configurare mai precisă ar fi: 
            // .Setup(c => c.Convert(originalAmount, "USD", "EUR")).Returns(expectedConvertedAmount);
            _mockConverter.Setup(c => c.Convert(It.IsAny<float>(), "USD", "EUR")).Returns(expectedConvertedAmount);

            string result = _bankService.GetTransactionInfo(originalAmount, _mockConverter.Object);

            // Verificarea rezultatului
            string expectedString = $"Original Amount: {originalAmount} USD. Converted Amount: {expectedConvertedAmount} EUR.";
            Assert.That(result, Is.EqualTo(expectedString));
        }

        // Test 2: Simulează un transfer cu conversie (metodă din Account extinsă)
        [Test]
        public void TransferFundsWithConversion_UsesConverter_TransfersCorrectAmount()
        {
            float sourceInitial = 200f;
            float transferAmount = 100f;
            float expectedConvertedAmount = 500f;
            var source = new Account(sourceInitial);
            var destination = new Account(0);

            // **Setup Moq**: 100 EUR sunt convertiți în 500 RON
            _mockConverter
                .Setup(c => c.Convert(transferAmount, "EUR", "RON"))
                .Returns(expectedConvertedAmount);

            source.TransferFundsWithConversion(destination, transferAmount, "EUR", "RON", _mockConverter.Object);

            // Verificarea stării conturilor
            Assert.That(source.Balance, Is.EqualTo(sourceInitial - transferAmount)); // 100 EUR
            Assert.That(destination.Balance, Is.EqualTo(expectedConvertedAmount)); // 500 RON

            // **Verify Moq**: Verificăm că metoda Convert a fost apelată exact o dată cu parametrii așteptați
            _mockConverter.Verify(
                c => c.Convert(transferAmount, "EUR", "RON"), 
                Times.Once, 
                "Convert method was not called exactly once with the expected parameters.");
        }

        // Test 3: Verifică dacă o metodă (simulată) a fost apelată (LogConversionRate)
        [Test]
        public void LogConversionRate_ShouldCallConvertMethod()
        {
            // Act: Rulăm metoda care ar trebui să apeleze un alt serviciu (Mock-ul nostru)
            _bankService.LogConversionRate(_mockConverter.Object);

            // **Verify Moq**: Verificăm că metoda Convert a fost apelată
            _mockConverter.Verify(
                c => c.Convert(It.IsAny<float>(), It.IsAny<string>(), It.IsAny<string>()), 
                Times.Once, 
                "Convert method was not called.");
        }
        
        // Test Suplimentar: Simulează aruncarea unei excepții
        [Test]
        public void TransferFundsWithConversion_ConverterThrowsException_TestCatchesException()
        {
            float transferAmount = 100f;
            var source = new Account(200f);
            var destination = new Account(0);

            // **Setup Moq**: Simulăm că metoda Convert aruncă o excepție (eșecul serviciului extern)
            _mockConverter
                .Setup(c => c.Convert(It.IsAny<float>(), It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new NotSupportedException("Conversion service is down."));

            // Act & Assert: Verificăm că metoda de transfer propagă excepția simulată
            Assert.Throws<NotSupportedException>(() => 
                source.TransferFundsWithConversion(destination, transferAmount, "USD", "XYZ", _mockConverter.Object));
        }
    }
}
