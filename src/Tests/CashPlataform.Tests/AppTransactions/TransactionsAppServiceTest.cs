using Bogus;
using CashPlataform.Application.AppTransactions;
using CashPlataform.Domain.Common.Enums;
using CashPlataform.Domain.Entities;
using CashPlataform.Domain.Interfaces.Repository;
using CashPlataform.Tests.AppAccount.Generate;
using CashPlataform.Tests.AppTransactions.Generate;
using Excel.Adapter.Interfaces;
using Excel.Adapter.Models;
using FluentAssertions;
using Marraia.Notifications;
using Marraia.Notifications.Handlers;
using Marraia.Notifications.Models;
using Marraia.Notifications.Models.Enum;
using MediatR;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace CashPlataform.Tests.AppTransactions
{
    public class TransactionsAppServiceTest
    {
        private const int defaultReceived = 1;
        private const int defaultCount = 1;
        private const int zeroReceived = 0;

        private ILogger<SmartNotification> logger;
        private IReportDaily reportDaily;
        private ICurrentAccountRepository currentAccountRepository;
        private IReleaseStatementRepository releaseStatementRepository;
        private INotificationHandler<DomainNotification> subNotificationHandler;
        private DomainNotificationHandler domainNotificationHandler;
        private SmartNotification smartNotification;
        private TransactionsAppService transactionsAppService;
        private Faker faker;

        public TransactionsAppServiceTest()
        {
            this.logger = Substitute.For<ILogger<SmartNotification>>();
            this.currentAccountRepository = Substitute.For<ICurrentAccountRepository>();
            this.releaseStatementRepository = Substitute.For<IReleaseStatementRepository>();
            this.reportDaily = Substitute.For<IReportDaily>();
            domainNotificationHandler = new DomainNotificationHandler();
            this.subNotificationHandler = domainNotificationHandler;
            this.smartNotification = new SmartNotification(this.logger, this.subNotificationHandler);
            faker = new Faker();

            this.transactionsAppService = new TransactionsAppService(this.smartNotification,
                                                                     this.currentAccountRepository,
                                                                     this.releaseStatementRepository,
                                                                     this.reportDaily);
        }

       
        [Theory]
        [InlineData(10000, 10)]
        [InlineData(10000, 100)]
        [InlineData(10000, 1000)]
        [InlineData(10000, 10000)]
        public async Task AddTransactForCurrentAccount_Operation_Credit_With_Success(decimal balance, decimal creditValue)
        {
            // Arrange
            var currentAccount = GenerateFakerCurrentAccount.CreateCurrentAccount(balance);
            var transactInput = GenerateFakerReleaseStatement.CreateTransactInputInput(OperationType.Credit, creditValue);

            this.currentAccountRepository
                    .GetByIdAsync(currentAccount.Id)
                    .Returns(currentAccount);

            // Act
            var result = await this.transactionsAppService
                                    .AddTransactForCurrentAccountAsync(currentAccount.Id, transactInput)
                                    .ConfigureAwait(false);

            // Assert
            result.Id.Should().NotBeEmpty();
            result.CurrentAccountId.Should().Be(currentAccount.Id);
            result.Description.Should().NotBeEmpty();
            result.OperationType.Should().Be(OperationType.Credit);
            result.Balance.Should().Be(currentAccount.Balance);
            result.Value.Should().Be(transactInput.Value);
            result.Date.Should().BeSameDateAs(DateTime.Now);

            await this.currentAccountRepository
                        .Received(defaultReceived)
                        .GetByIdAsync(Arg.Any<Guid>())
                        .ConfigureAwait(false);

            await this.currentAccountRepository
                        .Received(defaultReceived)
                        .UpdateBalanceAsync(Arg.Any<CurrentAccount>())
                        .ConfigureAwait(false);

            await this.releaseStatementRepository
                        .Received(defaultReceived)
                        .InsertAsync(Arg.Any<ReleaseStatement>())
                        .ConfigureAwait(false);
        }

        [Theory]
        [InlineData(OperationType.Credit)]
        [InlineData(OperationType.Debit)]
        public async Task AddTransactForCurrentAccount_Operations_But_Account_Not_Found(OperationType operationType)
        {
            // Arrange
            var balance = faker.Finance.Amount();
            var creditValue = faker.Finance.Amount();
            var currentAccount = GenerateFakerCurrentAccount.CreateCurrentAccount(balance);
            var transactInput = GenerateFakerReleaseStatement.CreateTransactInputInput(operationType, creditValue);

            this.currentAccountRepository
                    .GetByIdAsync(currentAccount.Id)
                    .Returns(default(CurrentAccount));

            // Act
            var result = await this.transactionsAppService
                                    .AddTransactForCurrentAccountAsync(currentAccount.Id, transactInput)
                                    .ConfigureAwait(false);

            // Assert
            result
                .Should()
                .BeNull();

            await this.currentAccountRepository
                        .Received(zeroReceived)
                        .UpdateBalanceAsync(Arg.Any<CurrentAccount>())
                        .ConfigureAwait(false);

            await this.releaseStatementRepository
                        .Received(zeroReceived)
                        .InsertAsync(Arg.Any<ReleaseStatement>())
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
                .Be("Conta corrente não foi encontrada, para realizar a operação");
        }

        [Theory]
        [InlineData(10000, 10)]
        [InlineData(10000, 100)]
        [InlineData(10000, 1000)]
        [InlineData(10000, 10000)]
        public async Task AddTransactForCurrentAccount_Operation_Debit_With_Success(decimal balance, decimal debitValue)
        {
            // Arrange
            var currentAccount = GenerateFakerCurrentAccount.CreateCurrentAccount(balance);
            var transactInput = GenerateFakerReleaseStatement.CreateTransactInputInput(OperationType.Debit, debitValue);

            this.currentAccountRepository
                    .GetByIdAsync(currentAccount.Id)
                    .Returns(currentAccount);

            // Act
            var result = await this.transactionsAppService
                                    .AddTransactForCurrentAccountAsync(currentAccount.Id, transactInput)
                                    .ConfigureAwait(false);

            // Assert
            result.Id.Should().NotBeEmpty();
            result.CurrentAccountId.Should().Be(currentAccount.Id);
            result.Description.Should().NotBeEmpty();
            result.OperationType.Should().Be(OperationType.Debit);
            result.Balance.Should().Be(currentAccount.Balance);
            result.Value.Should().Be(transactInput.Value);
            result.Date.Should().BeSameDateAs(DateTime.Now);

            await this.currentAccountRepository
                        .Received(defaultReceived)
                        .GetByIdAsync(Arg.Any<Guid>())
                        .ConfigureAwait(false);

            await this.currentAccountRepository
                        .Received(defaultReceived)
                        .UpdateBalanceAsync(Arg.Any<CurrentAccount>())
                        .ConfigureAwait(false);

            await this.releaseStatementRepository
                        .Received(defaultReceived)
                        .InsertAsync(Arg.Any<ReleaseStatement>())
                        .ConfigureAwait(false);
        }

        [Theory]
        [InlineData(0, 1)]
        [InlineData(-10, 10)]
        [InlineData(10, 100)]
        [InlineData(100, 1000)]
        [InlineData(1000, 10000)]
        public async Task AddTransactForCurrentAccount_Operation_Debit_But_Balance_Zero_Or_Negative_Not_Execution_Transaction(decimal balance, decimal debitValue)
        {
            // Arrange
            var currentAccount = GenerateFakerCurrentAccount.CreateCurrentAccount(balance);
            var transactInput = GenerateFakerReleaseStatement.CreateTransactInputInput(OperationType.Debit, debitValue);

            this.currentAccountRepository
                    .GetByIdAsync(currentAccount.Id)
                    .Returns(currentAccount);

            // Act
            var result = await this.transactionsAppService
                                    .AddTransactForCurrentAccountAsync(currentAccount.Id, transactInput)
                                    .ConfigureAwait(false);

            // Assert
            result
                .Should()
                .BeNull();

            await this.currentAccountRepository
                        .Received(defaultReceived)
                        .GetByIdAsync(Arg.Any<Guid>())
                        .ConfigureAwait(false);

            await this.currentAccountRepository
                        .Received(zeroReceived)
                        .UpdateBalanceAsync(Arg.Any<CurrentAccount>())
                        .ConfigureAwait(false);

            await this.releaseStatementRepository
                        .Received(zeroReceived)
                        .InsertAsync(Arg.Any<ReleaseStatement>())
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
                .Be($"Você não tem saldo suficiente para fazer esse débito de {debitValue.ToString("c")}! Pois seu saldo é de {balance.ToString("c")}");
        }

        [Fact]
        public async Task GetReportConsolidateDaily_With_Sucess()
        {
            // Arrange
            var currentAccount = GenerateFakerCurrentAccount.CreateCurrentAccount(decimal.Zero);

            this.currentAccountRepository
                    .GetByIdAsync(currentAccount.Id)
                    .Returns(currentAccount);

            var list = new List<ReleaseStatement>();
            var accountId = Guid.NewGuid();
            list.Add(new ReleaseStatement(accountId, 1000, "TESTE", 10, OperationType.Credit));
            list.Add(new ReleaseStatement(accountId, 1010, "TESTE", 100, OperationType.Credit));
            list.Add(new ReleaseStatement(accountId, 1110, "TESTE", 200, OperationType.Credit));
            list.Add(new ReleaseStatement(accountId, 1110, "TESTE", 10, OperationType.Debit));
            list.Add(new ReleaseStatement(accountId, 1100, "TESTE", 100, OperationType.Debit));
            list.Add(new ReleaseStatement(accountId, 990, "TESTE", 200, OperationType.Debit));

            this.reportDaily
                    .GenerateReportDailyAsync(Arg.Any<AccountReport>())
                    .Returns(new byte[] { 0, 1, 2 });

            this.releaseStatementRepository
                    .GetReleaseStatementsByPeriodAsync(Arg.Any<Guid>(), Arg.Any<DateTime>(), Arg.Any<DateTime>())
                    .Returns(list);

            // Act
            var result = await this.transactionsAppService
                                    .GetReportConsolidateDailyAsync(currentAccount.Id, DateTime.Now, DateTime.Now)
                                    .ConfigureAwait(false);

            // Assert
            result.Should().BeOfType<byte[]>();
            result.Should().NotBeNullOrEmpty();
        }

        [Fact]
        public async Task GetReportConsolidateDaily_But_Account_Not_Found()
        {
            // Arrange
            var balance = faker.Finance.Amount();
            var currentAccount = GenerateFakerCurrentAccount.CreateCurrentAccount(balance);

            this.currentAccountRepository
                    .GetByIdAsync(currentAccount.Id)
                    .Returns(default(CurrentAccount));

            // Act
            var result = await this.transactionsAppService
                                    .GetReportConsolidateDailyAsync(currentAccount.Id, DateTime.Now, DateTime.Now)
                                    .ConfigureAwait(false);

            // Assert
            result
                .Should()
                .BeNull();

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
                .Be("Conta corrente não foi encontrada, para realizar a operação");
        }
    }
}
