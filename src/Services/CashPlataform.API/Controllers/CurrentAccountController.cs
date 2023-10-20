using CashPlataform.Application.AppAccount.Interfaces;
using CashPlataform.Application.AppTransactions.Input;

using Marraia.Notifications.Base;
using Marraia.Notifications.Models;

using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CashPlataform.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CurrentAccountController : BaseController
    {
        private readonly ICurrentAccountAppService _currentAccountAppService;

        public CurrentAccountController(INotificationHandler<DomainNotification> notification,
                                        ICurrentAccountAppService currentAccountAppService)
            : base(notification)
        {
            _currentAccountAppService = currentAccountAppService;
        }

        [Authorize(AuthenticationSchemes = "BasicAuthentication")]
        [HttpPost]
        [ProducesResponseType(201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> PostAsync([FromBody] CurrentAccountInput currentAccountInput)
        {
            var account = await _currentAccountAppService
                                    .AddNewCurrentAccountAsync(currentAccountInput)
                                    .ConfigureAwait(false);

            return CreatedContent("", account);
        }
    }
}
