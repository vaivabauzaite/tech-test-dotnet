using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.Validators
{
    /// <summary>
    /// Validates Chaps (Clearing House Automated Payment  System) payments.
    /// The account must exist, have Chaps enabled and be in Live status.
    /// </summary>
    public class ChapsPaymentValidator : IPaymentValidator
    {
        public PaymentScheme PaymentScheme => PaymentScheme.Chaps;

        public bool IsValid(Account account, MakePaymentRequest request)
        {
            return account != null
                && account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.Chaps)
                && account.Status == AccountStatus.Live;
        }
    }
}
