using CashPlataform.Application.AppTransactions.Input;
using CashPlataform.Domain.Entities;

namespace CashPlataform.Application.AppTransactions.Interfaces
{
    public interface ITransactionsAppService
    {
        Task<ReleaseStatement> AddTransactForCurrentAccountAsync(Guid id, TransactInput transactInput);
        Task<byte[]> GetReportConsolidateDailyAsync(Guid accountId, DateTime from, DateTime to);
    }
}
