using Microsoft.Extensions.DependencyInjection;

public class BankServiceFactory
{
    private readonly IServiceProvider _serviceProvider;

    public BankServiceFactory(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    public IBankService Create(string bankId)
    {
        return bankId switch
        {
            "monobank" => _serviceProvider.GetRequiredService<MonobankService>(),
            "privatbank" => _serviceProvider.GetRequiredService<PrivatBankService>(),
            _ => throw new ArgumentException("Invalid bank ID")
        };
    }
}
