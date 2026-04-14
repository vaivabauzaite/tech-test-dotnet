using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Services.Validators;
using ClearBank.DeveloperTest.Types;
using Moq;
namespace ClearBank.DeveloperTest.Tests;

public class PaymentServiceTests
{
    private Mock<IAccountDataStore> _accountDataStoreMock = null!;
    private PaymentService _sut = null!;

    [Before(Test)]
    public void Setup()
    {
        // Mock IAccountDataStore since it "interacts with the database"
        _accountDataStoreMock = new Mock<IAccountDataStore>();
        var validators = new List<IPaymentValidator>
        {
            new BacsPaymentValidator(),
            new FasterPaymentsPaymentValidator(),
            new ChapsPaymentValidator()
        };
        _sut = new PaymentService(_accountDataStoreMock.Object, validators);
    }

    [Test]
    public async Task MakePayment_Bacs_ValidAccount_ReturnsTrue()
    {
        // Arrange
        var account = new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs, Balance = 100m };
        _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>())).Returns(account);

        // Act
        var result = _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Bacs, Amount = 50m });

        // Assert
        await Assert.That(result.Success).IsTrue();
        await Assert.That(account.Balance).IsEqualTo(50m);
    }


    [Test]
    public async Task MakePayment_FasterPayments_ValidAccount_ReturnsTrue()
    {
        // Arrange
        var account = new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments, Balance = 100m };
        _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>())).Returns(account);

        // Act
        var result = _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.FasterPayments, Amount = 50m });

        // Assert
        await Assert.That(result.Success).IsTrue();
        await Assert.That(account.Balance).IsEqualTo(50m);
    }

    [Test]
    public async Task MakePayment_FasterPayments_ValidAccount_ReturnsFalse()
    {
        // Arrange
        _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>()))
            .Returns(new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments, Balance = 10m });

        // Act
        var result = _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.FasterPayments, Amount = 50m });

        // Assert
        await Assert.That(result.Success).IsFalse();
    }

    [Test]
    public async Task MakePayment_Chaps_ValidAccount_ReturnsTrue()
    {
        // Arrange
        _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>()))
            .Returns(new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps, Status = AccountStatus.Live });

        // Act
        var result = _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Chaps });

        // Assert
        await Assert.That(result.Success).IsTrue();
    }

    [Test]
    public async Task MakePayment_Chaps_ValidAccount_ReturnsFalse_WhenAccountIsDisabled()
    {
        // Arrange
        _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>()))
            .Returns(new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps, Status = AccountStatus.Disabled });

        // Act
        var result = _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Chaps });

        // Assert
        await Assert.That(result.Success).IsFalse();
    }

    [Test]
    public async Task MakePayment_AccountNotFound_ReturnsFalse()
    {
        // Arrange
        _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>())).Returns((Account?)null!);

        // Act
        var result = _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Bacs });

        // Assert
        await Assert.That(result.Success).IsFalse();
    }
}
