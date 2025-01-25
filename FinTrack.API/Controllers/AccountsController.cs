using Microsoft.AspNetCore.Mvc;

namespace FinTrack.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountsController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var accounts = await _accountService.GetAccountsAsync();
            return Ok(accounts);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var account = await _accountService.GetAccountByIdAsync(id);
            if (account == null) return NotFound();

            return Ok(account);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Account account)
        {
            await _accountService.AddAccountAsync(account);
            return CreatedAtAction(nameof(GetById), new { id = account.Id }, account);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(string id, Account account)
        {
            if (id != account.Id) return BadRequest();

            await _accountService.UpdateAccountAsync(account);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _accountService.DeleteAccountAsync(id);
            return NoContent();
        }
    }
}
