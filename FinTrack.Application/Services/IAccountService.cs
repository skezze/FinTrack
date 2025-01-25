public interface IAccountService
{
    Task<IEnumerable<Account>> GetAccountsAsync();
    Task<Account?> GetAccountByIdAsync(int id);
    Task AddAccountAsync(Account account);
    Task UpdateAccountAsync(Account account);
    Task DeleteAccountAsync(int id);
}
