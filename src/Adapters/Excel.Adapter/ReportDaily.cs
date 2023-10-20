using Excel.Adapter.Interfaces;
using Excel.Adapter.Models;
using OfficeOpenXml;
using System.Data;
using System.Drawing;

namespace Excel.Adapter
{
    public class ReportDaily : IReportDaily
    {
        const int LineTitle = 4;
        const int LineInit = 5;
        const int DateColumn = 1;
        const int CreditColumn = 2;
        const int DescriptionColumn = 2;
        const int DebitColumn = 3;
        const int ValueColumn = 3;
        const int BalanceColumn = 4;
        public byte[] GenerateReportDailyAsync(AccountReport accountReport)
        {
            using (var excelPackage = new ExcelPackage())
            {
                var worksheet = excelPackage.Workbook.Worksheets.Add("Consolidado Diário");
                var worksheetRelease = excelPackage.Workbook.Worksheets.Add("Extrato");

                GenerateHeaderReport(worksheet, accountReport.AccountName, accountReport.Balance);
                GenerateHeaderReport(worksheetRelease, accountReport.AccountName, accountReport.Balance);

                GenerateTitleColumnConsolidateReport(worksheet);
                GenerateTitleColumnReleaseReport(worksheetRelease);

                GenerateConsolidateReport(worksheet, accountReport.ConsolidateAccount);
                GenerateReleaseReport(worksheetRelease, accountReport.ConsolidateRelease);

                worksheet.Cells.AutoFitColumns();
                worksheetRelease.Cells.AutoFitColumns();

                return excelPackage.GetAsByteArray();
            }
        }

        private void GenerateHeaderReport(ExcelWorksheet worksheet, string accountName, decimal balance)
        {
            worksheet.Cells[1, 1].Style.Font.Bold = true;
            worksheet.Cells[1, 1].Value = "Conta";

            worksheet.Cells[1, 2].Style.Font.Bold = true;
            worksheet.Cells[1, 2].Value = accountName;

            worksheet.Cells[2, 1].Style.Font.Bold = true;
            worksheet.Cells[2, 1].Value = "Saldo Atual";

            worksheet.Cells[2, 2].Style.Font.Bold = true;
            worksheet.Cells[2, 2].Style.Font.Color.SetColor(Color.Blue);
            worksheet.Cells[2, 2].Value = balance.ToString("c");
        }

        private void GenerateTitleColumnConsolidateReport(ExcelWorksheet worksheet)
        {
            worksheet.Cells[LineTitle, DateColumn].Style.Font.Bold = true;
            worksheet.Cells[LineTitle, DateColumn].Value = "Data";
            worksheet.Cells[LineTitle, CreditColumn].Style.Font.Bold = true;
            worksheet.Cells[LineTitle, CreditColumn].Value = "Crédito";
            worksheet.Cells[LineTitle, DebitColumn].Style.Font.Bold = true;
            worksheet.Cells[LineTitle, DebitColumn].Value = "Débito";
            worksheet.Cells[LineTitle, BalanceColumn].Style.Font.Bold = true;
            worksheet.Cells[LineTitle, BalanceColumn].Value = "Saldo Diário";
        }

        private void GenerateTitleColumnReleaseReport(ExcelWorksheet worksheet)
        {
            worksheet.Cells[LineTitle, DateColumn].Style.Font.Bold = true;
            worksheet.Cells[LineTitle, DateColumn].Value = "Data";
            worksheet.Cells[LineTitle, DescriptionColumn].Style.Font.Bold = true;
            worksheet.Cells[LineTitle, DescriptionColumn].Value = "Descrição";
            worksheet.Cells[LineTitle, ValueColumn].Style.Font.Bold = true;
            worksheet.Cells[LineTitle, ValueColumn].Value = "Valor";
        }

        private void GenerateConsolidateReport(ExcelWorksheet worksheet, IEnumerable<ConsolidateAccount> consolidateAccount)
        {
            var countLines = consolidateAccount.Count() + LineInit;

            for (var line = LineInit; line < countLines; line++)
            {
                var consolidateOrder = consolidateAccount
                                            .OrderByDescending(conso => conso.Date)
                                            .ToList();

                var lineAccountReport = line - LineInit;
                var dateTransaction = consolidateOrder[lineAccountReport].Date;
                var creditValueTransaction = consolidateOrder[lineAccountReport].Credit;
                var debitValueTransaction = consolidateOrder[lineAccountReport].Debit;
                var balanceValue = consolidateOrder[lineAccountReport].Balance;

                worksheet.Cells[line, DateColumn].Value = dateTransaction.ToString("dd/MM/yyyy");

                worksheet.Cells[line, CreditColumn].Value = creditValueTransaction.ToString("c");
                worksheet.Cells[line, CreditColumn].Style.Font.Color.SetColor(Color.Green);
                worksheet.Cells[line, CreditColumn].Style.Font.Bold = true;

                worksheet.Cells[line, DebitColumn].Value = $"- {debitValueTransaction.ToString("c")}";
                worksheet.Cells[line, DebitColumn].Style.Font.Color.SetColor(Color.Red);
                worksheet.Cells[line, DebitColumn].Style.Font.Bold = true;

                worksheet.Cells[line, BalanceColumn].Value = balanceValue.ToString("c");
                worksheet.Cells[line, BalanceColumn].Style.Font.Color.SetColor(Color.Blue);
                worksheet.Cells[line, BalanceColumn].Style.Font.Bold = true;
            }
        }

        private void GenerateReleaseReport(ExcelWorksheet worksheet, IEnumerable<ConsolidateRelease> consolidateReleases)
        {
            var countLines = consolidateReleases.Count() + LineInit;

            for (var line = LineInit; line < countLines; line++)
            {
                var consolidateOrder = consolidateReleases
                                            .OrderByDescending(conso => conso.Date)
                                            .ToList();

                var lineAccountReport = line - LineInit;
                var dateTransaction = consolidateOrder[lineAccountReport].Date;
                var descriptionTransaction = consolidateOrder[lineAccountReport].Description;
                var valueTransaction = consolidateOrder[lineAccountReport].Value;
                var operationTransaction = consolidateOrder[lineAccountReport].Operation;

                worksheet.Cells[line, DateColumn].Value = dateTransaction.ToString("dd/MM/yyyy");
                worksheet.Cells[line, DescriptionColumn].Value = descriptionTransaction;
                

                if (operationTransaction == "cred")
                {
                    worksheet.Cells[line, ValueColumn].Value = valueTransaction.ToString("c");
                    worksheet.Cells[line, ValueColumn].Style.Font.Color.SetColor(Color.Green);
                    worksheet.Cells[line, ValueColumn].Style.Font.Bold = true;
                }
                else
                {
                    worksheet.Cells[line, ValueColumn].Value = $"- {valueTransaction.ToString("c")}";
                    worksheet.Cells[line, ValueColumn].Style.Font.Color.SetColor(Color.Red);
                    worksheet.Cells[line, ValueColumn].Style.Font.Bold = true;
                }
            }
        }
    }
}
