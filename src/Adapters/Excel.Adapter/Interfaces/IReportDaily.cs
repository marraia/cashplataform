using Excel.Adapter.Models;

namespace Excel.Adapter.Interfaces
{
    public interface IReportDaily
    {
        byte[] GenerateReportDailyAsync(AccountReport accountReport);
    }
}
