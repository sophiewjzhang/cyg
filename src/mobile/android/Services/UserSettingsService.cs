using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using android.Services.Abstractions;
using models;
using System.Threading.Tasks;

namespace android.Services
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