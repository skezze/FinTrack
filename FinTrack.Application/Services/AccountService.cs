public class AccountService : IAccountService
{
    private readonly IRepository<Account> _accountRepository;

    public AccountService(IRepository<Account> accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<IEnumerable<Account>> GetAccountsAsync()
    {
        return await _accountRepository.GetAllAsync();
    }

    public async Task<Account?> GetAccountByIdAsync(int id)
    {
        return await _accountRepository.GetByIdAsync(id);
    }

    public async Task AddAccountAsync(Account account)
    {
        await _accountRepository.AddAsync(account);
    }

    public async Task UpdateAccountAsync(Account account)
    {
        await _accountRepository.UpdateAsync(account);
    }

    public async Task DeleteAccountAsync(int id)
    {
        await _accountRepository.DeleteAsync(id);
    }
}
