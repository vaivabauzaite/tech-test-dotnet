using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Services
{
    /// <summary>
    /// Defines the contract for processing outbound payments from a debtor account.
    /// </summary>
    public interface IPaymentService
    {
        MakePaymentResult MakePayment(MakePaymentRequest request);
    }
}
