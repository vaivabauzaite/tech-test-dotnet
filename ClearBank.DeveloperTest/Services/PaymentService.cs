using ClearBank.DeveloperTest.Data;
using ClearBank.DeveloperTest.Services.Validators;
using ClearBank.DeveloperTest.Types;
using System.Collections.Generic;
using System.Linq;

namespace ClearBank.DeveloperTest.Services
{
    /// <summary>
    /// Orchestrates the payment flow: fetches the debtor account, delegates validation to the
    /// appropriate scheme validator, and — if valid — deducts the amount and persists the updated account.
    /// </summary>
    public class PaymentService : IPaymentService
    {
        private readonly IAccountDataStore _accountDataStore;
        private readonly IEnumerable<IPaymentValidator> _validators;

        public PaymentService(IAccountDataStore accountDataStore, IEnumerable<IPaymentValidator> validators)
        {
            _accountDataStore = accountDataStore;
            _validators = validators;
        }

        public MakePaymentResult MakePayment(MakePaymentRequest request)
        {
            Account account = _accountDataStore.GetAccount(request.DebtorAccountNumber);
            IPaymentValidator validator = _validators.FirstOrDefault(v => v.PaymentScheme == request.PaymentScheme);

            var result = new MakePaymentResult
            {
                Success = validator != null && validator.IsValid(account, request)
            };

            if (result.Success)
            {
                account.Balance -= request.Amount;
                _accountDataStore.UpdateAccount(account);

                // TODO: Ensure creditor is credited after debtor is deducted.
                // Both updates should be wrapped in a transaction to ensure consistency.
            }

            return result;
        }
    }
}
