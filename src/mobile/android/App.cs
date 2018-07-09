using android.Configuration;
using Android.App;
using Microsoft.Extensions.Configuration;
using Autofac;
using System.IO;
using Android.Runtime;
using System;
using System.Threading.Tasks;
using Android.OS;
using Android.Widget;
using System.Net.Http;
using Newtonsoft.Json;
using Akavache;

namespace android
{
    [Application]
    public class App : Application
    {
        public static IContainer Container { private set; get; }

        public App(IntPtr handle, JniHandleOwnership ownerShip) : base(handle, ownerShip)
        {
        }

        public async override void OnCreate()
        {
            base.OnCreate();

            var configuration = new ConfigurationBuilder()
            .AddJsonFile(new ResourceFileProvider(), "android.appsettings.json", false, false)
            .Build()
            .Get<AndroidConfiguration>();
            var startup = new Startup(configuration, this);
            Container = startup.GetContainer();


            // TODO: add logging
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
        }
    }
}