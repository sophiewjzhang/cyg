using Android.App;
using Android.Content;
using Autofac;
using services.abstractions;
using System.Threading.Tasks;

namespace android
{
    [Activity(Theme = "@style/MyTheme.Splash", MainLauncher = true, NoHistory = true)]
    public class SplashActivity : Activity
    {
        static readonly string TAG = "X:" + typeof(SplashActivity).Name;
        ICacheService cacheService;

        // Launches the startup task
        protected async override void OnResume()
        {
            base.OnResume();
            cacheService = App.Container.Resolve<ICacheService>();
            await cacheService.UpdateCacheStatusIfRequired();
            Task startupWork = new Task(() => { Startup(); });
            startupWork.Start();
        }

        // Simulates background work that happens behind the splash screen
        private async void Startup()
        {
            StartActivity(new Intent(Application.Context, typeof(TripsActivity)));
        }
    }
}