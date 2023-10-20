using CashPlataform.Domain.Interfaces.Repository;
using CashPlataform.Infrastructure.Repositories.Repository;

using Marraia.MongoDb.Configurations;

using Microsoft.Extensions.DependencyInjection;

namespace CashPlataform.Infrastructure.IoC.Repositories
{
    internal class RepositoryBootstrapper
    {
        internal void ChildServiceRegister(IServiceCollection service)
        {
            service.AddMongoDb();
            service.AddScoped<ICurrentAccountRepository, CurrentAccountRepository>();
            service.AddScoped<IReleaseStatementRepository, ReleaseStatementRepository>();
        }
    }
}
