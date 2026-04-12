namespace ClearBank.DeveloperTest.Types
{
    /// <summary>
    /// Represents the operational status of a bank account.
    /// Only Live accounts are eligible for all outbound payment schemes.
    /// </summary>
    public enum AccountStatus
    {
        Live,
        Disabled,
        InboundPaymentsOnly
    }
}
