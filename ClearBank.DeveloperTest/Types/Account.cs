namespace ClearBank.DeveloperTest.Types
{
    /// <summary>
    /// Represents a bank account, including its current balance, status, and the payment schemes it is permitted to use.
    /// </summary>
    public class Account
    {
        public string AccountNumber { get; set; }
        public decimal Balance { get; set; }
        public AccountStatus Status { get; set; }
        public AllowedPaymentSchemes AllowedPaymentSchemes { get; set; }
    }
}
