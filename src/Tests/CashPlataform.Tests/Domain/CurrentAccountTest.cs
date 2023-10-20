using Bogus;
using CashPlataform.Domain.Common.Enums;
using CashPlataform.Tests.AppAccount.Generate;
using FluentAssertions;

namespace CashPlataform.Tests.Domain
{
    public class CurrentAccountTest
    {
        [Theory]
        [InlineData(5)]
        [InlineData(25)]
        [InlineData(625)]
        [InlineData(390625)]
        public async Task Operation_Credit_With_Success(int qtdTransaction)
        {
            // Arrange
            var currentAccount = GenerateFakerCurrentAccount.CreateCurrentAccount(decimal.Zero);
            var operationList = new List<decimal>();
            var faker = new Faker();

            for (int i = 0; i < qtdTransaction; i++)
            {
                operationList.Add(faker.Finance.Amount());
            }

            // Act
            foreach (var creditValue in operationList)
            {
                currentAccount.Transact(creditValue, OperationType.Credit);
            }

            // Assert
            currentAccount.Balance.Should().Be(operationList.Sum());
        }

        [Theory]
        [InlineData(5)]
        [InlineData(25)]
        [InlineData(625)]
        [InlineData(390625)]
        public async Task Operation_Debit_With_Success(int qtdTransaction)
        {
            // Arrange
            var currentAccount = GenerateFakerCurrentAccount.CreateCurrentAccount(decimal.MaxValue);
            var operationList = new List<decimal>();
            var faker = new Faker();

            for (int i = 0; i < qtdTransaction; i++)
            {
                operationList.Add(faker.Finance.Amount());
            }

            // Act
            foreach (var creditValue in operationList)
            {
                currentAccount.Transact(creditValue, OperationType.Debit);
            }

            // Assert
            currentAccount.Balance.Should().BeGreaterThanOrEqualTo(operationList.Sum());
        }
    }
}
