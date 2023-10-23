using Bogus;
using CashPlataform.Tests.Adapter.Generate;
using Excel.Adapter;
using Excel.Adapter.Models;
using FluentAssertions;


namespace CashPlataform.Tests.Adapter
{
    public class ExcelAdapterTest
    {
        private ReportDaily reportDaily;

        public ExcelAdapterTest()
        {
            this.reportDaily = new ReportDaily();
        }

        [Fact (Skip = "Github Action não funciona a instalação do Gdpi")]
        public async Task Adapter_GetReportConsolidateDaily_With_Sucess()
        {
            // Arrange
            var faker = new Faker();
            var currentAccount = GenerateFakerAccountReport.CreateAccountReport(faker.Random.Int(0, 1000));


            // Act
            var result =  this.reportDaily
                              .GenerateReportDailyAsync(currentAccount);

            // Assert
            result.Should().BeOfType<byte[]>();
            result.Should().NotBeNullOrEmpty();
        }


        [Fact]
        public async Task Adapter_GetReportConsolidateDaily_Not_Sucess()
        {
            // Arrange
            var currentAccount = default(AccountReport);


            // Act
            var result = this.reportDaily
                              .GenerateReportDailyAsync(currentAccount);

            // Assert
            result.Should().BeNull();
        }
    }
}
