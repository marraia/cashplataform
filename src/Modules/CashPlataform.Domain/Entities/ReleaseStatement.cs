using CashPlataform.Domain.Common.Enums;
using Marraia.MongoDb.Core;

namespace CashPlataform.Domain.Entities
{
    public class ReleaseStatement : Entity<Guid>
    {
        public ReleaseStatement(){}
        public ReleaseStatement(Guid currentAccountId,
                                decimal balance,
                                string description,
                                decimal value,
                                OperationType operationType)
        {
            Id = Guid.NewGuid();
            CurrentAccountId = currentAccountId;
            Balance = balance;
            Description = description;
            Value = value;
            OperationType = operationType;
            Date = DateTime.Now;
        }

        public Guid CurrentAccountId { get; private set; }
        public string Description { get; private set; }
        public decimal Value { get; private set; }
        public decimal Balance { get; private set; }
        public OperationType OperationType { get; private set; }
        public DateTime Date { get; private set; }
    }
}
