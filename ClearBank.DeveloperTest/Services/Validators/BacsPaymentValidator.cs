using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.Validators
{
    /// <summary>
    /// Bankers' Automated Clearing Services (Bacs) is a UK-based payment scheme that processes direct debits and direct credits.
    /// Validates Bacs payments. The account must exist and have Bacs enabled.
    /// </summary>
    public class BacsPaymentValidator : IPaymentValidator
    {
        public PaymentScheme PaymentScheme => PaymentScheme.Bacs;

        public bool IsValid(Account account, MakePaymentRequest request)
        {
            return account != null && account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Bacs);
        }
    }
}
