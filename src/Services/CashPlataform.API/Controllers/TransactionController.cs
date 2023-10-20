using CashPlataform.Application.AppTransactions.Input;
using CashPlataform.Application.AppTransactions.Interfaces;

using Marraia.Notifications.Base;
using Marraia.Notifications.Models;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashPlataform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionController : BaseController
    {
        private readonly ITransactionsAppService _transactionsAppService;

        public TransactionController(INotificationHandler<DomainNotification> notification,
                                     ITransactionsAppService transactionsAppService)
            : base(notification)
        {
            _transactionsAppService = transactionsAppService;
        }

        [Authorize(AuthenticationSchemes = "BasicAuthentication")]
        [HttpPost]
        [Route("{accountId}")]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostTransactionAsync([FromRoute] Guid accountId, [FromBody] TransactInput transactInput)
        {
            var account = await _transactionsAppService
                                    .AddTransactForCurrentAccountAsync(accountId, transactInput)
                                    .ConfigureAwait(false);

            return CreatedContent("", account);
        }

        [Authorize(AuthenticationSchemes = "BasicAuthentication")]
        [HttpGet]
        [Route("{accountId}/Report")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> GetAsync([FromRoute] Guid accountId, [FromQuery] DateTime from, [FromQuery] DateTime to)
        {
            var fileArray = await _transactionsAppService
                                    .GetReportConsolidateDailyAsync(accountId, from, to)
                                    .ConfigureAwait(false);

            if (fileArray.Count() > 0)
                return File(fileArray, "application/vnd.ms-excel", $"{accountId}-{DateTime.Now.ToString("yyyy-MM-yy hh:mm:ss")}.xls");

            return OkOrNoContent(fileArray);
        }
    }
}
