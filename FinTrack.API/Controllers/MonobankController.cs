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
        public async Task<IActionResult> GetStatementForAMonthAsync()
        {
            var utcNow = DateTime.UtcNow;
            var utcNowTimestamp = utcNow.ToTimestampLong();
            var utc30DaysAgo = utcNow.AddDays(-30).ToTimestampLong();

            var statement = await monobankService.GetStatementAsync(from: utc30DaysAgo, to: utcNowTimestamp, accountId: "0");
            
            return Ok(statement);
        }

        [HttpGet]
        public async Task<IActionResult> GetClientInfo()
        {
            return Ok(await monobankService.GetClientInfoAsync());
        }

        [HttpGet]
        public IActionResult GetTransactionsAsync()
        {
            return Ok(dbContext.MonobankTransactions);
        }
    }
}
