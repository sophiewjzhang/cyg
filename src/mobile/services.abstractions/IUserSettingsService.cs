using models;
using System.Threading.Tasks;

namespace services.abstractions
{
    public interface IUserSettingsService
    {
        void SaveUserSettings(UserSettings settings);
        Task<UserSettings> LoadUserSettings();
    }
}