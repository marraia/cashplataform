using Excel.Adapter.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Excel.Adapter.Configurations
{
    public static class ExcelConfigurationsExtensions
    {
        public static IServiceCollection AddExcelReport(this IServiceCollection service)
        {
            service.AddScoped<IReportDaily, ReportDaily>();

            return service;
        }
    }
}
