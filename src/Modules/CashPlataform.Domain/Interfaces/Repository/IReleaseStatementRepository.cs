using CashPlataform.Domain.Entities;
using Marraia.MongoDb.Repositories.Interfaces;

namespace CashPlataform.Domain.Interfaces.Repository
{
    public interface IReleaseStatementRepository : IRepositoryBase<ReleaseStatement, Guid>
    {
        Task<IEnumerable<ReleaseStatement>> GetReleaseStatementsByPeriodAsync(Guid accountId, DateTime from, DateTime to);
    }
}
