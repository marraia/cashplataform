using CashPlataform.Domain.Common.Enums;

namespace CashPlataform.Application.AppTransactions.Input
{
    public class TransactInput
    {
        public string Description { get; set; }
        public decimal Value { get; set; }
        public OperationType Operation { get; set; }
    }
}
