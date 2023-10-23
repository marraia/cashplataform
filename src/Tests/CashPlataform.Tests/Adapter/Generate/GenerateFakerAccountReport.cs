using Bogus;
using Excel.Adapter.Models;

namespace CashPlataform.Tests.Adapter.Generate
{
    internal class GenerateFakerAccountReport
    {
        public static AccountReport CreateAccountReport(int qtd)
        {
            var account = new Faker<AccountReport>("pt_BR")
                                .StrictMode(true)
                                .RuleFor(c => c.AccountName, f => f.Company.CompanyName())
                                .RuleFor(c => c.Balance, f => f.Finance.Amount())
                                .RuleFor(c => c.ConsolidateAccount, CreateConsolidateAccount(qtd))
                                .RuleFor(c => c.ConsolidateRelease, CreateConsolidateRelease(qtd))
                                .Generate();

            return account;
        }

        public static IEnumerable<ConsolidateAccount> CreateConsolidateAccount(int qtd)
        {
            var consolidate = new Faker<ConsolidateAccount>("pt_BR")
                .StrictMode(false)
                .RuleFor(c => c.Date, f => f.Date.Between(DateTime.Now, f.Date.Future()))
                .RuleFor(c => c.Debit, f => f.Finance.Amount())
                .RuleFor(c => c.Credit, f => f.Finance.Amount())
                .RuleFor(c => c.Balance, f => f.Finance.Amount())
                .Generate(qtd);

            return consolidate;
        }

        public static IEnumerable<ConsolidateRelease> CreateConsolidateRelease(int qtd)
        {
            var consolidate = new Faker<ConsolidateRelease>("pt_BR")
                .StrictMode(false)
                .RuleFor(c => c.Date, f => f.Date.Between(DateTime.Now, f.Date.Future()))
                .RuleFor(c => c.Value, f => f.Finance.Amount())
                .RuleFor(c => c.Description, f => f.Commerce.ProductDescription())
                .Generate(qtd);

            return consolidate;
        }
    }
}
