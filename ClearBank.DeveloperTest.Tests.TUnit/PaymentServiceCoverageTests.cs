using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Services.Validators;
using ClearBank.DeveloperTest.Types;
using Moq;
using TUnit.Assertions.Extensions;

namespace ClearBank.DeveloperTest.Tests.TUnit;

public class PaymentServiceCoverageTests
{
    private Mock<IAccountDataStore> _accountDataStoreMock = null!;
    private PaymentService _sut = null!;

    [Before(Test)]
    public void Setup()
    {
        _accountDataStoreMock = new Mock<IAccountDataStore>();
        var validators = new List<IPaymentValidator>
        {
            new BacsPaymentValidator(),
            new FasterPaymentsPaymentValidator(),
            new ChapsPaymentValidator()
        };
        _sut = new PaymentService(_accountDataStoreMock.Object, validators);
    }

    // --- Boundary: FasterPayments exact balance ---

    [Test]
    public async Task MakePayment_FasterPayments_BalanceExactlyEqualsAmount_ReturnsTrue()
    {
        _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>()))
            .Returns(new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments, Balance = 50m });

        var result = _sut.MakePayment(new MakePaymentRequest
        {
            PaymentScheme = PaymentScheme.FasterPayments,
            Amount = 50m
        });

        await Assert.That(result.Success).IsTrue();
    }

    // --- Chaps: InboundPaymentsOnly status ---

    [Test]
    public async Task MakePayment_Chaps_AccountInboundPaymentsOnly_ReturnsFalse()
    {
        _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>()))
            .Returns(new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.InboundPaymentsOnly
            });

        var result = _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Chaps });

        await Assert.That(result.Success).IsFalse();
    }

    // --- Correct account number is used when fetching ---

    [Test]
    public void MakePayment_UsesDebtorAccountNumber_ToFetchAccount()
    {
        const string debtorAccountNumber = "ACC-123";
        _accountDataStoreMock.Setup(s => s.GetAccount(debtorAccountNumber))
            .Returns(new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs, Balance = 100m });

        _sut.MakePayment(new MakePaymentRequest
        {
            PaymentScheme = PaymentScheme.Bacs,
            DebtorAccountNumber = debtorAccountNumber
        });

        _accountDataStoreMock.Verify(s => s.GetAccount(debtorAccountNumber), Times.Once);
    }

    // --- Chaps: balance is deducted on success ---

    [Test]
    public async Task MakePayment_Chaps_Success_DeductsAmountFromBalance()
    {
        var account = new Account
        {
            AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps,
            Status = AccountStatus.Live,
            Balance = 200m
        };
        _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>())).Returns(account);

        _sut.MakePayment(new MakePaymentRequest
        {
            PaymentScheme = PaymentScheme.Chaps,
            Amount = 75m
        });

        await Assert.That(account.Balance).IsEqualTo(125m);
    }

    // --- Unrecognised payment scheme ---

    [Test]
    public async Task MakePayment_UnknownPaymentScheme_ReturnsFalse()
    {
        _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>()))
            .Returns(new Account
            {
                AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs | AllowedPaymentSchemes.FasterPayments | AllowedPaymentSchemes.Chaps,
                Status = AccountStatus.Live,
                Balance = 100m
            });

        var result = _sut.MakePayment(new MakePaymentRequest
        {
            PaymentScheme = (PaymentScheme)99
        });

        await Assert.That(result.Success).IsFalse();
    }

    // --- Parameterised: all non-Live statuses fail Chaps ---

    [Test]
    [Arguments(AccountStatus.Disabled)]
    [Arguments(AccountStatus.InboundPaymentsOnly)]
    public async Task MakePayment_Chaps_NonLiveStatus_ReturnsFalse(AccountStatus status)
    {
        _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>()))
            .Returns(new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps, Status = status });

        var result = _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Chaps });

        await Assert.That(result.Success).IsFalse();
    }
}
