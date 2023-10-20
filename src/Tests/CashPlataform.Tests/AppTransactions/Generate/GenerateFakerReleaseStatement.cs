using Bogus;
using CashPlataform.Application.AppTransactions.Input;
using CashPlataform.Domain.Common.Enums;

namespace CashPlataform.Tests.AppTransactions.Generate
{
    internal class GenerateFakerReleaseStatement
    {
        public static TransactInput CreateTransactInputInput(OperationType operation, decimal value)
        {
            var transact = new Faker<TransactInput>("pt_BR")
                                .StrictMode(true)
                                .RuleFor(c => c.Description, f => f.Finance.Account())
                                .RuleFor(c => c.Value, value)
                                .RuleFor(c => c.Operation, operation)
                                .Generate();

            return transact;
        }
    }
}
