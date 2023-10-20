using CashPlataform.Application.AppTransactions.Input;
using CashPlataform.Domain.Entities;

namespace CashPlataform.Application.AppAccount.Interfaces
{
    public interface ICurrentAccountAppService
    {
        Task<CurrentAccount> AddNewCurrentAccountAsync(CurrentAccountInput currentAccountInput);
    }
}
