using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services.Validators
{
    /// <summary>
    /// Defines the contract for validating a payment request against the rules of a specific payment scheme.
    /// Each implementation handles exactly one scheme.
    /// </summary>
    public interface IPaymentValidator
    {
        PaymentScheme PaymentScheme { get; }
        bool IsValid(Account account, MakePaymentRequest request);
    }
}
