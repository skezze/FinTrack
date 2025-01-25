using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/banks")]
public class BankController : ControllerBase
{
    private readonly BankServiceFactory _bankServiceFactory;

    public BankController(BankServiceFactory bankServiceFactory)
    {
        _bankServiceFactory = bankServiceFactory;
    }

    [HttpPost("sync-transactions")]
    public async Task<IActionResult> SyncTransactions(string bankId)
    {
        var bankService = _bankServiceFactory.Create(bankId);
        await bankService.SyncTransactionsAsync();

        return Ok("Transactions synced successfully.");
    }

    [HttpGet("{bankId}/accounts")]
    public async Task<IActionResult> GetAccounts(string bankId)
    {
        var bankService = _bankServiceFactory.Create(bankId);
        var accounts = await bankService.GetAccountsAsync();

        return Ok(accounts);
    }
}
