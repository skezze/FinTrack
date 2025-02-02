using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/account")]
//[Authorize]
public class AccountController : ControllerBase
{
    private readonly OpenBankingService _openBankingService;
    public AccountController(OpenBankingService openBankingService)
    {
        _openBankingService = openBankingService;
    }  
    [HttpGet("accounts/{userId}")]
    public async Task<IActionResult> GetAccounts(string userId)
    {
        await _openBankingService.GetBankAccountsAsync(userId);
        return Ok("Accounts fetched");
    }
}