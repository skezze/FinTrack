public interface IBankService
{
    Task SyncTransactionsAsync();
    Task<IEnumerable<Account>> GetAccountsAsync();
    Task<decimal> GetBalanceAsync();
}
