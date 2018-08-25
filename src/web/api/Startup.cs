using api.Controllers;
using dal;
using dal.Abstractions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public async void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddMemoryCache();

            var connectionString = Configuration.GetConnectionString("Api");
            services.AddTransient<IStopRepository, StopRepository>(x => new StopRepository(connectionString));
            services.AddTransient<IRouteRepository, RouteRepository>(x => new RouteRepository(connectionString));
            services.AddTransient<ITripRepository, TripRepository>(x => new TripRepository(connectionString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public async void Configure(IApplicationBuilder app, IHostingEnvironment env, IRouteRepository routeRepository, IStopRepository stopRepository, IMemoryCache cache)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseMvc();
            var routes = await routeRepository.GetTrainRoutes();
            foreach (var route in routes)
            {
                var stops = await stopRepository.GetStopsByRoute(route.RouteId);
                cache.Set(route.RouteId, stops, TimeSpan.FromDays(90));
            }
        }
    }
}
