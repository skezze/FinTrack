public class OpenBankingService
{
    public async Task<List<BankAccount>> GetBankAccountsAsync(string userId)
    {
        // Моковые данные вместо API Open Banking
        await Task.Delay(500); // Имитация сетевого запроса

        return new List<BankAccount>
        {
            new BankAccount { Id = "acc_123", UserId = userId, Balance = 1000.50m, Currency = "USD" },
            new BankAccount { Id = "acc_456", UserId = userId, Balance = 250.75m, Currency = "EUR" }
        };
    }

    public async Task<List<Transaction>> GetTransactionsAsync(string accountId)
    {
        // Моковые данные транзакций
        await Task.Delay(500); // Имитация задержки API

        return new List<Transaction>
        {
            new Transaction { Id = 1, UserId = "user_123", Amount = -50, Date = DateTime.UtcNow.AddDays(-1), Category = "Food" },
            new Transaction { Id = 2, UserId = "user_123", Amount = -20, Date = DateTime.UtcNow.AddDays(-2), Category = "Transport" },
            new Transaction { Id = 3, UserId = "user_123", Amount = 200, Date = DateTime.UtcNow.AddDays(-5), Category = "Salary" }
        };
    }
}