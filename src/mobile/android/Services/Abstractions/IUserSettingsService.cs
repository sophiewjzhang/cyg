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
using models;
using System.Threading.Tasks;

namespace android.Services.Abstractions
{
    public interface IUserSettingsService
    {
        void SaveUserSettings(UserSettings settings);
        Task<UserSettings> LoadUserSettings();
    }
}