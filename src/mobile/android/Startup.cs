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
using Microsoft.Extensions.Configuration;
using android.Configuration;
using Autofac;
using android.Services.Abstractions;
using android.Services;
using Microsoft.Extensions.Caching.Memory;
using Akavache;
using Akavache.Sqlite3;

namespace android
{
    public class Startup
    {
        public AndroidConfiguration Configuration { get; }

        public Startup(AndroidConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IContainer GetContainer()
        {
            var builder = new ContainerBuilder(); 
            builder.RegisterType<MemoryCache>().As<IMemoryCache>().WithParameter("optionsAccessor", new MemoryCacheOptions());
            BlobCache.ApplicationName = Configuration.CacheDatabaseName;
            builder.RegisterType<CacheService>().As<ICacheService>().WithParameter("inMemoryExpirationInHours", Configuration.InMemoryCacheExpirationInHours).WithParameter("permanentExpirationInDays", Configuration.PermanentCacheExpirationInDays);
            builder.RegisterType<RouteDataService>().As<IRouteDataService>().WithParameter("baseUrl", Configuration.ApiBaseUrl);
            builder.RegisterType<StopDataService>().As<IStopDataService>().WithParameter("baseUrl", Configuration.ApiBaseUrl);
            builder.RegisterType<TripDataService>().As<ITripDataService>().WithParameter("baseUrl", Configuration.ApiBaseUrl);
            return builder.Build();
        }
    }
}