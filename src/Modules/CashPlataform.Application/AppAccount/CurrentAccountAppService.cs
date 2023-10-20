using CashPlataform.Application.AppAccount.Interfaces;
using CashPlataform.Application.AppTransactions.Input;
using CashPlataform.Domain.Entities;
using CashPlataform.Domain.Interfaces.Repository;

using Marraia.Notifications.Interfaces;
using Marraia.Notifications.Validations;

namespace CashPlataform.Application.AppAccount
{
    public class CurrentAccountAppService : EntityValidator, ICurrentAccountAppService
    {
        private readonly ISmartNotification _smartNotification;
        private readonly ICurrentAccountRepository _currentAccountRepository;

        public CurrentAccountAppService(ISmartNotification smartNotification,
                                        ICurrentAccountRepository currentAccountRepository)
            : base(smartNotification)
        {
            _smartNotification = smartNotification;
            _currentAccountRepository = currentAccountRepository;
        }
        public async Task<CurrentAccount> AddNewCurrentAccountAsync(CurrentAccountInput currentAccountInput)
        {
            var currentAccount = new CurrentAccount(currentAccountInput.AccountName);

            var validateFields = currentAccount.Validate();
            if (!validateFields.IsValid)
            {
                NotifyValidationErrors(validateFields);
                return default;
            }

            var existsAccount = await _currentAccountRepository
                                        .GetByNameAsync(currentAccountInput.AccountName)
                                        .ConfigureAwait(false);

            if (existsAccount != null)
            {
                _smartNotification
                    .NewNotificationBadRequest($"Conta corrente de nome {existsAccount.AccountName} já existe! Por favor tente com outro nome");

                return default;
            }

            await _currentAccountRepository
                    .InsertAsync(currentAccount)
                    .ConfigureAwait(false);

            return currentAccount;
        }
    }
}
