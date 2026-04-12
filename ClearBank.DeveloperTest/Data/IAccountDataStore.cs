using ClearBank.DeveloperTest.Types;

namespace ClearBank.DeveloperTest.Data
{
    /// <summary>
    /// Defines the contract for retrieving and persisting account data.
    /// </summary>
    public interface IAccountDataStore
    {
        Account GetAccount(string accountNumber);
        void UpdateAccount(Account account);
    }
}
