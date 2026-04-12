using System;

namespace ClearBank.DeveloperTest.Types
{
    /// <summary>
    /// Carries the details of an outbound payment request, including the accounts involved,
    /// the amount, the payment date, and which scheme should be used to process it.
    /// </summary>
    public class MakePaymentRequest
    {
        // TODO: Ensure creditor is credited after debtor is deducted
        public string CreditorAccountNumber { get; set; }

        public string DebtorAccountNumber { get; set; }

        public decimal Amount { get; set; }

        public DateTime PaymentDate { get; set; }

        public PaymentScheme PaymentScheme { get; set; }
    }
}
