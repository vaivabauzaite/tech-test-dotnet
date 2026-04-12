using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services;
using ClearBank.DeveloperTest.Services.Validators;
using ClearBank.DeveloperTest.Types;
using Moq;
using System.Collections.Generic;
using Xunit;

namespace ClearBank.DeveloperTest.Tests
{
    public class PaymentServiceTests
    {
        private readonly Mock<IAccountDataStore> _accountDataStoreMock;
        private readonly PaymentService _sut;

        public PaymentServiceTests()
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

        // --- Bacs ---

        [Fact]
        public void MakePayment_Bacs_AccountNotFound_ReturnsFalse()
        {
            _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>())).Returns((Account)null);

            var result = _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Bacs });

            Assert.False(result.Success);
        }

        [Fact]
        public void MakePayment_Bacs_SchemeNotAllowed_ReturnsFalse()
        {
            _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>()))
                .Returns(new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments });

            var result = _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Bacs });

            Assert.False(result.Success);
        }

        [Fact]
        public void MakePayment_Bacs_ValidAccount_ReturnsTrue()
        {
            _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>()))
                .Returns(new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs, Balance = 100m });

            var result = _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Bacs, Amount = 50m });

            Assert.True(result.Success);
        }

        // --- FasterPayments ---

        [Fact]
        public void MakePayment_FasterPayments_AccountNotFound_ReturnsFalse()
        {
            _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>())).Returns((Account)null);

            var result = _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.FasterPayments, Amount = 50m });

            Assert.False(result.Success);
        }

        [Fact]
        public void MakePayment_FasterPayments_SchemeNotAllowed_ReturnsFalse()
        {
            _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>()))
                .Returns(new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs, Balance = 100m });

            var result = _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.FasterPayments, Amount = 50m });

            Assert.False(result.Success);
        }

        [Fact]
        public void MakePayment_FasterPayments_InsufficientBalance_ReturnsFalse()
        {
            _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>()))
                .Returns(new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments, Balance = 10m });

            var result = _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.FasterPayments, Amount = 50m });

            Assert.False(result.Success);
        }

        [Fact]
        public void MakePayment_FasterPayments_ValidAccount_ReturnsTrue()
        {
            _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>()))
                .Returns(new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.FasterPayments, Balance = 100m });

            var result = _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.FasterPayments, Amount = 50m });

            Assert.True(result.Success);
        }

        // --- Chaps ---

        [Fact]
        public void MakePayment_Chaps_AccountNotFound_ReturnsFalse()
        {
            _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>())).Returns((Account)null);

            var result = _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Chaps });

            Assert.False(result.Success);
        }

        [Fact]
        public void MakePayment_Chaps_SchemeNotAllowed_ReturnsFalse()
        {
            _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>()))
                .Returns(new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs, Status = AccountStatus.Live });

            var result = _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Chaps });

            Assert.False(result.Success);
        }

        [Fact]
        public void MakePayment_Chaps_AccountNotLive_ReturnsFalse()
        {
            _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>()))
                .Returns(new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps, Status = AccountStatus.Disabled });

            var result = _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Chaps });

            Assert.False(result.Success);
        }

        [Fact]
        public void MakePayment_Chaps_ValidAccount_ReturnsTrue()
        {
            _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>()))
                .Returns(new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.Chaps, Status = AccountStatus.Live });

            var result = _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Chaps });

            Assert.True(result.Success);
        }

        // --- Balance and update ---

        [Fact]
        public void MakePayment_Success_DeductsAmountFromBalance()
        {
            var account = new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs, Balance = 100m };
            _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>())).Returns(account);

            _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Bacs, Amount = 40m });

            Assert.Equal(60m, account.Balance);
        }

        [Fact]
        public void MakePayment_Success_CallsUpdateAccount()
        {
            var account = new Account { AllowedPaymentSchemes = AllowedPaymentSchemes.Bacs, Balance = 100m };
            _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>())).Returns(account);

            _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Bacs, Amount = 40m });

            _accountDataStoreMock.Verify(s => s.UpdateAccount(account), Times.Once);
        }

        [Fact]
        public void MakePayment_Failure_DoesNotCallUpdateAccount()
        {
            _accountDataStoreMock.Setup(s => s.GetAccount(It.IsAny<string>())).Returns((Account)null);

            _sut.MakePayment(new MakePaymentRequest { PaymentScheme = PaymentScheme.Bacs });

            _accountDataStoreMock.Verify(s => s.UpdateAccount(It.IsAny<Account>()), Times.Never);
        }
    }
}
