public interface IUserSettingsService
{
    Task<UserSettings?> GetUserSettingsAsync(string userId);
    Task<bool> UpdateUserSettingsAsync(UserSettings settings);
}