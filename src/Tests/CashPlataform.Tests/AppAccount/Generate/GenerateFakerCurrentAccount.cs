using Bogus;
using CashPlataform.Application.AppTransactions.Input;
using CashPlataform.Domain.Entities;

namespace CashPlataform.Tests.AppAccount.Generate
{
    internal class GenerateFakerCurrentAccount
    {
        public static CurrentAccountInput CreateCurrentAccountInput()
        {
            var account = new Faker<CurrentAccountInput>("pt_BR")
                                .StrictMode(true)
                                .RuleFor(c => c.AccountName, f => f.Company.CompanyName())
                                .Generate();

            return account;
        }

        public static CurrentAccount CreateCurrentAccount(decimal balance)
        {
            var account = new Faker<CurrentAccount>("pt_BR")
                                .StrictMode(true)
                                .RuleFor(c => c.Id, f => f.Random.Uuid())
                                .RuleFor(c => c.AccountName, f => f.Company.CompanyName())
                                .RuleFor(c => c.Balance, balance)
                                .Generate();

            return account;
        }

    }
}
