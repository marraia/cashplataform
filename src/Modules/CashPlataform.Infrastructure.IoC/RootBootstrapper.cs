using CashPlataform.Infrastructure.IoC.Application;
using CashPlataform.Infrastructure.IoC.Repositories;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace CashPlataform.Infrastructure.IoC
{
    public class RootBootstrapper
    {
        public void BootstrapperRegisterServices(IServiceCollection services, IConfiguration configuration)
        {
            new RepositoryBootstrapper().ChildServiceRegister(services);
            new ApplicationBootstrapper().ChildServiceRegister(services, configuration);
        }
    }
}
