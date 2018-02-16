using android.Configuration;
using Android.App;
using Microsoft.Extensions.Configuration;
using Autofac;
using System.IO;
using Android.Runtime;
using System;

namespace android
{
    [Application]
    public class App : Application
    {
        public static IContainer Container { private set; get; }

        public App(IntPtr handle, JniHandleOwnership ownerShip) : base(handle, ownerShip)
        {
        }

        public override void OnCreate()
        {
            base.OnCreate();

            var configuration = new ConfigurationBuilder()
           .AddJsonFile(new ResourceFileProvider(), "android.appsettings.json", false, false).Build().Get<AndroidConfiguration>();
            var startup = new Startup(configuration);
            Container = startup.GetContainer();
        }
    }
}