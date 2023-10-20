using CashPlataform.Application.AppAccount;
using CashPlataform.Application.AppAccount.Interfaces;
using CashPlataform.Application.AppTransactions;
using CashPlataform.Application.AppTransactions.Interfaces;

using Excel.Adapter.Configurations;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashPlataform.Infrastructure.IoC.Application
{
    internal class ApplicationBootstrapper
    {
        internal void ChildServiceRegister(IServiceCollection service, IConfiguration configuration)
        {
            service.AddExcelReport();
            service.AddScoped<ICurrentAccountAppService, CurrentAccountAppService>();
            service.AddScoped<ITransactionsAppService, TransactionsAppService>();
        }
    }
}
