using FinTrack.API;
using Microsoft.EntityFrameworkCore;

public class UserSettingsService : IUserSettingsService
{
    private readonly ApplicationDbContext _dbContext;

    public UserSettingsService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<UserSettings?> GetUserSettingsAsync(string userId)
    {
        return await _dbContext.UserSettings.FirstOrDefaultAsync(u => u.UserId == userId);
    }

    public async Task<bool> UpdateUserSettingsAsync(UserSettings settings)
    {
        _dbContext.UserSettings.Update(settings);
        return await _dbContext.SaveChangesAsync() > 0;
    }
}
