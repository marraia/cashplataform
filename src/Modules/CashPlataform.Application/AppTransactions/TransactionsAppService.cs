using CashPlataform.Application.AppTransactions.Input;
using CashPlataform.Application.AppTransactions.Interfaces;
using CashPlataform.Application.AppTransactions.Mapper;

using CashPlataform.Domain.Entities;
using CashPlataform.Domain.Interfaces.Repository;

using Excel.Adapter.Interfaces;

using Marraia.Notifications.Interfaces;

namespace CashPlataform.Application.AppTransactions
{
    public class TransactionsAppService : ITransactionsAppService
    {
        private readonly ISmartNotification _smartNotification;
        private readonly ICurrentAccountRepository _currentAccountRepository;
        private readonly IReleaseStatementRepository _releaseStatementRepository;
        private readonly IReportDaily _reportDaily;
        public TransactionsAppService(ISmartNotification smartNotification,
                                       ICurrentAccountRepository currentAccountRepository,
                                       IReleaseStatementRepository releaseStatementRepository,
                                       IReportDaily reportDaily)
        {   
            _smartNotification = smartNotification;
            _currentAccountRepository = currentAccountRepository;
            _releaseStatementRepository = releaseStatementRepository;
            _reportDaily = reportDaily;
        }

        public async Task<ReleaseStatement> AddTransactForCurrentAccountAsync(Guid accountId, TransactInput transactInput)
        {
            var currentAccount = await _currentAccountRepository
                                        .GetByIdAsync(accountId)
                                        .ConfigureAwait(false);
            if (currentAccount is null)
            {
                _smartNotification
                    .NewNotificationBadRequest("Conta corrente não foi encontrada, para realizar a operação");

                return default;
            }

            var executionTransact = currentAccount
                                        .Transact(transactInput.Value, 
                                                  transactInput.Operation);

            if (!executionTransact.Sucess)
            {
                _smartNotification
                    .NewNotificationBadRequest(executionTransact.Message);

                return default;
            }

            await _currentAccountRepository
                   .UpdateBalanceAsync(currentAccount)
                   .ConfigureAwait(false);

            var releaseStatement = new ReleaseStatement(currentAccount.Id,
                                                        currentAccount.Balance,
                                                        transactInput.Description, 
                                                        transactInput.Value,
                                                        transactInput.Operation);
            await _releaseStatementRepository
                    .InsertAsync(releaseStatement)
                    .ConfigureAwait(false);

            return releaseStatement;
        }

        public async Task<byte[]> GetReportConsolidateDailyAsync(Guid accountId, DateTime from, DateTime to)
        {
            var currentAccount = await _currentAccountRepository
                                        .GetByIdAsync(accountId)
                                        .ConfigureAwait(false);

            if (currentAccount is null)
            {
                _smartNotification
                    .NewNotificationBadRequest("Conta corrente não foi encontrada, para realizar a operação");

                return default;
            }

            var releaseStatement = await _releaseStatementRepository
                                            .GetReleaseStatementsByPeriodAsync(accountId, from, to)  
                                            .ConfigureAwait(false);

            var accountReport = releaseStatement.Map(currentAccount.AccountName, currentAccount.Balance);

            var reportGenerate = _reportDaily
                                    .GenerateReportDailyAsync(accountReport);

            return reportGenerate;
        }
    }
}
