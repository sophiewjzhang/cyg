using android.Services.Abstractions;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace android.Services
{
    public class TripDataService : BaseDataService, ITripDataService
    {
        private const string AllTripsCacheKey = "Trips";

        public TripDataService(ICacheService cacheService, string baseUrl) : base(cacheService, baseUrl)
        {
        }

        public async Task<IEnumerable<TripFromTo>> GetNextThreeTrips(string routeId, Stop from, Stop to)
        {
            var todaysTrips = await GetAsync<TripFromTo>($"{AllTripsCacheKey}:{routeId}:{DateTime.Now.Date}:{from}:{to}", $"trip/{from.StopId}/{to.StopId}/{routeId}/{DateTime.Now.Date}");
            return todaysTrips.OrderBy(x => x.From.DepartureTime).Where(x => x.From.DepartureTime > DateTime.Now.TimeOfDay).Take(3);
        }

        public async Task<IEnumerable<TripFromTo>> GetTripsFromTo(string routeId, DateTime date, Stop from, Stop to)
        {
            var dateFormat = date.ToString("yyyy-MM-dd");
            return await GetAsync<TripFromTo>($"{AllTripsCacheKey}:{routeId}:{dateFormat}:{from}:{to}", $"trip/{from.StopId}/{to.StopId}/{routeId}/{dateFormat}");
        }
    }
}