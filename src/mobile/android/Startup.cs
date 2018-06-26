using Akavache;
using android.Configuration;
using android.Services;
using android.Services.Abstractions;
using Android.Content;
using Android.Locations;
using Autofac;
using Microsoft.Extensions.Caching.Memory;
using System.Collections.Generic;

namespace android
{
    public class Startup
    {
        private readonly AndroidConfiguration configuration;
        private readonly Context context;

        public Startup(AndroidConfiguration configuration, Context context)
        {
            this.configuration = configuration;
            this.context = context;
        }

        public IContainer GetContainer()
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<MemoryCache>().As<IMemoryCache>().WithParameter("optionsAccessor", new MemoryCacheOptions());
            BlobCache.ApplicationName = configuration.CacheDatabaseName;
            builder.RegisterType<CacheService>().As<ICacheService>().WithParameter("inMemoryExpirationInHours", configuration.InMemoryCacheExpirationInHours).WithParameter("permanentExpirationInDays", configuration.PermanentCacheExpirationInDays);
            builder.RegisterType<RouteDataService>().As<IRouteDataService>().WithParameter("baseUrl", configuration.ApiBaseUrl)
                .WithParameter("apiTimeoutInSeconds", configuration.ApiTimeoutInSeconds);
            builder.RegisterType<StopDataService>().As<IStopDataService>().WithParameter("baseUrl", configuration.ApiBaseUrl)
                .WithParameter("apiTimeoutInSeconds", configuration.ApiTimeoutInSeconds);
            builder.RegisterType<TripDataService>().As<ITripDataService>().WithParameter("baseUrl", configuration.ApiBaseUrl)
                .WithParameter("apiTimeoutInSeconds", configuration.ApiTimeoutInSeconds);
            builder.RegisterType<UserSettingsService>().As<IUserSettingsService>();
            builder.RegisterType<LocationService>().As<ILocationService>();
            builder.RegisterInstance((LocationManager)context.GetSystemService(Context.LocationService));
            return builder.Build();
        }
    }
}