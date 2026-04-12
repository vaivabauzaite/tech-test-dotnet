namespace ClearBank.DeveloperTest.Types
{
    /// <summary>
    /// A bit flag enum representing which payment schemes an account is permitted to use.
    /// Multiple schemes can be combined on a single account.
    /// </summary>
    public enum AllowedPaymentSchemes
    {
        FasterPayments = 1 << 0,
        Bacs = 1 << 1,
        Chaps = 1 << 2
    }
}
