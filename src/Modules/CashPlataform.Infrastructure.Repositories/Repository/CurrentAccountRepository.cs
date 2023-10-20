using CashPlataform.Domain.Entities;
using CashPlataform.Domain.Interfaces.Repository;

using Marraia.MongoDb.Repositories;
using Marraia.MongoDb.Repositories.Interfaces;

using MongoDB.Driver;
using MongoDB.Driver.Linq;

namespace CashPlataform.Infrastructure.Repositories.Repository
{
    public class CurrentAccountRepository : MongoDbRepositoryStandard<CurrentAccount, Guid>, ICurrentAccountRepository
    {
        public CurrentAccountRepository(IMongoContext context) 
            : base(context)
        {
        }

        public async Task<CurrentAccount> GetByIdAsync(Guid id)
        {
            return await Collection
                            .AsQueryable()
                            .Where(currentAccount => currentAccount.Id == id)
                            .FirstOrDefaultAsync()
                            .ConfigureAwait(false);
        }

        public async Task<CurrentAccount> GetByNameAsync(string accountName)
        {
            return await Collection
                            .AsQueryable()
                            .Where(currentAccount => currentAccount.AccountName == accountName)
                            .FirstOrDefaultAsync()
                            .ConfigureAwait(false);
        }

        public async Task InsertAsync(CurrentAccount currentAccount)
        {
            await Collection
                    .InsertOneAsync(currentAccount)
                    .ConfigureAwait(false);
        }

        public async Task UpdateBalanceAsync(CurrentAccount currentAccount)
        {
            var filter = Builders<CurrentAccount>.Filter.Eq(x => x.Id, currentAccount.Id);

            var updateDefinition = Builders<CurrentAccount>
                                    .Update
                                    .Set(x => x.Balance, currentAccount.Balance);

            await Collection
                    .UpdateOneAsync(filter, updateDefinition)
                    .ConfigureAwait(false);
        }
    }
}
