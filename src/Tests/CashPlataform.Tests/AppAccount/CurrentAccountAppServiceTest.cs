using Bogus;
using CashPlataform.Application.AppAccount;
using CashPlataform.Application.AppTransactions;
using CashPlataform.Application.AppTransactions.Input;
using CashPlataform.Domain.Entities;
using CashPlataform.Domain.Interfaces.Repository;
using CashPlataform.Tests.AppAccount.Generate;
using Excel.Adapter.Interfaces;
using FluentAssertions;
using Marraia.Notifications;
using Marraia.Notifications.Handlers;
using Marraia.Notifications.Models;
using Marraia.Notifications.Models.Enum;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;


namespace CashPlataform.Tests.AppAccount
{
    public class CurrentAccountAppServiceTest
    {
        private const int defaultReceived = 1;
        private const int defaultCount = 1;
        private const int zeroReceived = 0;

        private ILogger<SmartNotification> logger;
        private ICurrentAccountRepository currentAccountRepository;
        private INotificationHandler<DomainNotification> subNotificationHandler;
        private DomainNotificationHandler domainNotificationHandler;
        private SmartNotification smartNotification;
        private CurrentAccountAppService currentAccountAppService;
        private Faker faker;

        public CurrentAccountAppServiceTest()
        {
            this.logger = Substitute.For<ILogger<SmartNotification>>();
            this.currentAccountRepository = Substitute.For<ICurrentAccountRepository>();

            domainNotificationHandler = new DomainNotificationHandler();
            this.subNotificationHandler = domainNotificationHandler;
            this.smartNotification = new SmartNotification(this.logger, this.subNotificationHandler);
            faker = new Faker();

            this.currentAccountAppService = new CurrentAccountAppService(this.smartNotification,
                                                                         this.currentAccountRepository);
        }

        [Fact]
        public async Task AddAccount_With_Success()
        {
            // Arrange
            var input = GenerateFakerCurrentAccount.CreateCurrentAccountInput();

            // Act
            var result = await this.currentAccountAppService
                                    .AddNewCurrentAccountAsync(input)
                                    .ConfigureAwait(false);

            // Assert
            await this.currentAccountRepository
                        .Received(zeroReceived)
                        .UpdateBalanceAsync(Arg.Any<CurrentAccount>())
                        .ConfigureAwait(false);

            await this.currentAccountRepository
                        .Received(defaultReceived)
                        .InsertAsync(Arg.Any<CurrentAccount>())
                        .ConfigureAwait(false);

            result.Id.Should().NotBeEmpty();
            result.Balance.Should().Be(decimal.Zero);
            result.AccountName.Should().Be(input.AccountName);
        }


        [Fact]
        public async Task AddAccount_But_AccountName_Exists()
        {
            // Arrange
            var input = GenerateFakerCurrentAccount.CreateCurrentAccountInput();
            var currentAccount = GenerateFakerCurrentAccount.CreateCurrentAccount(decimal.Zero);

            this.currentAccountRepository
                    .GetByNameAsync(input.AccountName)
                    .Returns(currentAccount);

            // Act
            var result = await this.currentAccountAppService
                                    .AddNewCurrentAccountAsync(input)
                                    .ConfigureAwait(false);

            // Assert
            result
                .Should()
                .BeNull();

            await this.currentAccountRepository
                        .Received(zeroReceived)
                        .UpdateBalanceAsync(Arg.Any<CurrentAccount>())
            .ConfigureAwait(false);

            await this.currentAccountRepository
                        .Received(zeroReceived)
                        .InsertAsync(Arg.Any<CurrentAccount>())
                        .ConfigureAwait(false);

            domainNotificationHandler
                .GetNotifications()
                .Should()
                .HaveCount(defaultCount);
            domainNotificationHandler?
                .GetNotifications()?
                .FirstOrDefault()?
                .DomainNotificationType
                .Should()
                .Be(DomainNotificationType.BadRequest);

            domainNotificationHandler?
                .GetNotifications()?
                .FirstOrDefault()?
                .Value
                .Should()
                .Be($"Conta corrente de nome {currentAccount.AccountName} já existe! Por favor tente com outro nome");
        }

        [Fact]
        public async Task AddAccount_But_AccountName_Is_Required()
        {
            // Arrange
            var input = new CurrentAccountInput();
            var currentAccount = GenerateFakerCurrentAccount.CreateCurrentAccount(decimal.Zero);

            // Act
            var result = await this.currentAccountAppService
                                    .AddNewCurrentAccountAsync(input)
                                    .ConfigureAwait(false);

            // Assert
            result
                .Should()
                .BeNull();

            await this.currentAccountRepository
                        .Received(zeroReceived)
                        .UpdateBalanceAsync(Arg.Any<CurrentAccount>())
                        .ConfigureAwait(false);

            await this.currentAccountRepository
                        .Received(zeroReceived)
                        .InsertAsync(Arg.Any<CurrentAccount>())
                        .ConfigureAwait(false);

            domainNotificationHandler
                .GetNotifications()
                .Should()
                .HaveCount(defaultCount);

            domainNotificationHandler?
                .GetNotifications()?
                .FirstOrDefault()?
                .DomainNotificationType
                .Should()
                .Be(DomainNotificationType.BadRequest);

            domainNotificationHandler?
                .GetNotifications()?
                .FirstOrDefault()?
                .Value
                .Should()
                .Be("O nome da conta é obrigatório");
        }

    }
}
