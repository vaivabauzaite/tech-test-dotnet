namespace ClearBank.DeveloperTest.Types
{
    /// <summary>
    /// Represents the UK payment scheme to be used when processing a payment.
    /// Each scheme has different rules around settlement speed, balance requirements, and account eligibility.
    /// </summary>
    public enum PaymentScheme
    {
        FasterPayments,
        Bacs,
        Chaps
    }
}
