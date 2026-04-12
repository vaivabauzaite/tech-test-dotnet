using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.Validators
{
    /// <summary>
    /// Validates FasterPayments payments. FasterPayments is the UK's near-instant payment
    /// The account must exist, have FasterPayments enabled and hold sufficient balance 
    /// to cover the requested amount.
    /// </summary>
    public class FasterPaymentsPaymentValidator : IPaymentValidator
    {
        public PaymentScheme PaymentScheme => PaymentScheme.FasterPayments;

        public bool IsValid(Account account, MakePaymentRequest request)
        {
            return account != null
                && account.AllowedPaymentSchemes.HasFlag(AllowedPaymentSchemes.FasterPayments)
                && account.Balance >= request.Amount;
        }
    }
}
