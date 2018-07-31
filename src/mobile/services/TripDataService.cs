using DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using services.abstractions;

namespace services
{
    public class TripDataService : BaseDataService, ITripDataService
    {
        public TripDataService(ICacheService cacheService, string baseUrl, int apiTimeoutInSeconds) : base(cacheService, baseUrl, apiTimeoutInSeconds)
        {
        }

        public async Task<IEnumerable<TripFromTo>> GetNextThreeTrips(string routeId, Stop from, Stop to)
        {
            return await GetNextThreeTrips(routeId, from.StopId, to.StopId);
        }

        public async Task<IEnumerable<TripFromTo>> GetNextThreeTrips(string routeId, string from, string to)
        {
            var todaysTrips = await GetTripsFromTo(routeId, DateTime.Now, from, to);
            return todaysTrips.OrderBy(x => x.From.DepartureTime).Where(x => x.From.DepartureTime > DateTime.Now.TimeOfDay).Take(3);
        }

        public async Task<IEnumerable<TripFromTo>> GetNextThreeTrips(string routeId, DateTime dateTime, string from, string to)
        {
            var todaysTrips = await GetTripsFromTo(routeId, dateTime, from, to);
            return todaysTrips.OrderBy(x => x.From.DepartureTime).Where(x => x.From.DepartureTime > dateTime.TimeOfDay).Take(3);
        }

        public async Task<IEnumerable<TripFromTo>> GetTripsFromTo(string routeId, DateTime date, Stop from, Stop to)
        {
            return await GetTripsFromTo(routeId, date, from.StopId, to.StopId);
        }

        public async Task<IEnumerable<TripFromTo>> GetTripsFromTo(string routeId, DateTime date, string from, string to)
        {
            var dateFormat = date.ToString("yyyy-MM-dd");
            return await GetCachedAsync<TripFromTo>($"trip/{from}/{to}/{routeId}/{dateFormat}");
        }

        public async Task<Tuple<DateTime, DateTime>> GetAvailableDates()
        {
            return await GetEntityFromApiAsync<Tuple<DateTime, DateTime>>(new Uri(baseUrl, "trip/dates"));
        }

        public async Task<IEnumerable<TripFromTo>> GetEligibleTrips(string routeId, DateTime date, string from, string to)
        {
            var dateFormat = date.ToString("yyyy-MM-dd");
            return await GetAsync<TripFromTo>($"trip/eligible/{from}/{to}/{routeId}/{dateFormat}");
        }
    }
}