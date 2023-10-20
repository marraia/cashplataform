using CashPlataform.Domain.Common.Enums;
using CashPlataform.Domain.Entities;

using Excel.Adapter.Models;

namespace CashPlataform.Application.AppTransactions.Mapper
{
    internal static class MappingReportConsolidate
    {
        internal static AccountReport Map(this IEnumerable<ReleaseStatement> releaseStatements, string accountName, decimal balance)
        {
            var accountReport = new AccountReport();
            accountReport.AccountName = accountName;
            accountReport.Balance = balance;

            var groupingDaily = releaseStatements
                                .GroupBy(moviment => moviment.Date.ToString("yyyy-MM-dd"))
                                .Select(moviment => new ConsolidateAccount()
                                {
                                    Date = moviment.FirstOrDefault().Date,
                                    Credit = moviment.Where(x => x.OperationType == OperationType.Credit).Sum(x => x.Value),
                                    Debit = moviment.Where(x => x.OperationType == OperationType.Debit).Sum(x => x.Value),
                                    Balance = moviment.LastOrDefault().Balance
                                }).ToList();

            accountReport.ConsolidateAccount = groupingDaily.ToList();

            foreach(var item in releaseStatements)
            {
                accountReport.ConsolidateRelease.Add(new ConsolidateRelease()
                {
                    Date = item.Date,
                    Description = item.Description,
                    Value = item.Value,
                    Operation = item.OperationType == OperationType.Credit ? "cred" : "deb"
                });
            }

            return accountReport;
        }
    }
}
