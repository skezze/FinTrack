using FinTrack.API.Extensions;
using FinTrack.API.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    //[Authorize]
    public class MonobankController: ControllerBase
    {
        private readonly IMonobankService monobankService;
        private readonly ApplicationDbContext dbContext;

        public MonobankController(
            IMonobankService monobankService,
            ApplicationDbContext dbContext
            )
        {
            this.monobankService = monobankService;
            this.dbContext = dbContext;
        }

        [HttpGet]
        public async Task<IActionResult> GetClientInfo()
        {
            return Ok(await monobankService.GetClientInfoAsync());
        }

        [HttpGet]
        public async Task<IActionResult> GetTransactionsAsync(List<string> accountIds)
        {
            return Ok(dbContext.MonobankTransactions.Where(i => accountIds.Contains(i.AccountId)));
        }
    }
}
