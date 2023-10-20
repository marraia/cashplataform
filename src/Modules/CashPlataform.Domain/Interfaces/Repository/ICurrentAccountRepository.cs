using CashPlataform.Domain.Entities;

namespace CashPlataform.Domain.Interfaces.Repository
{
    public interface ICurrentAccountRepository
    {
        Task InsertAsync(CurrentAccount currentAccount);
        Task<CurrentAccount> GetByIdAsync(Guid id);
        Task<CurrentAccount> GetByNameAsync(string accountName);
        Task UpdateBalanceAsync(CurrentAccount currentAccount);
    }
}
