using CashPlataform.Domain.Entities;
using CashPlataform.Domain.Interfaces.Repository;

using Marraia.MongoDb.Repositories;
using Marraia.MongoDb.Repositories.Interfaces;

using MongoDB.Driver;

namespace CashPlataform.Infrastructure.Repositories.Repository
{
    public class ReleaseStatementRepository : MongoDbRepositoryBase<ReleaseStatement, Guid>, IReleaseStatementRepository
    {
        public ReleaseStatementRepository(IMongoContext context) 
            : base(context)
        {
        }

        public async Task<IEnumerable<ReleaseStatement>> GetReleaseStatementsByPeriodAsync(Guid accountId, DateTime from, DateTime to)
        {
            var filterBuilder = Builders<ReleaseStatement>.Filter;
            var filter = filterBuilder.Eq("CurrentAccountId", accountId) &
                         filterBuilder.Gte("Date", from) &
                         filterBuilder.Lte("Date", to);

          return await Collection
                        .Find(filter)
                        .ToListAsync()
                        .ConfigureAwait(false);
        }
    }
}
