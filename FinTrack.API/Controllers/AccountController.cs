using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/account")]
public class AccountController : ControllerBase
{
    private readonly OpenBankingService _openBankingService;
    public AccountController(OpenBankingService openBankingService)
    {
        _openBankingService = openBankingService;
    }
    [HttpGet("accounts")]
    public IActionResult GetAccounts()
    {
        _openBankingService.GetBankAccounts();
        return Ok("Accounts fetched");
    }
}