using models;
using services.abstractions;
using System.Threading.Tasks;

namespace services
{
    public class UserSettingsService : IUserSettingsService
    {
        private ICacheService cacheService;
        private const string UserSettingsCacheKey = "userSettings";

        public UserSettingsService(ICacheService cacheService)
        {
            this.cacheService = cacheService;
        }

        public async Task<UserSettings> LoadUserSettings()
        {
            return await cacheService.GetObjectFromUserCacheAsync<UserSettings>(UserSettingsCacheKey);
        }

        public void SaveUserSettings(UserSettings settings)
        {
            cacheService.AddObjectToUserCache(UserSettingsCacheKey, settings);
        }
    }
}