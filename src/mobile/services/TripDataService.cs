using DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using services.abstractions;
using dto.Extensions;

namespace services
{
    public class TripDataService : BaseDataService, ITripDataService
    {
        private int eligibilityDaysAvailable;

        public TripDataService(ICacheService cacheService, string baseUrl, int apiTimeoutInSeconds, int eligibilityDaysAvailable) : base(cacheService, baseUrl, apiTimeoutInSeconds)
        {
            this.eligibilityDaysAvailable = eligibilityDaysAvailable;
        }

        public async Task<IEnumerable<TripFromTo>> GetNextThreeTrips(string routeId, DateTime dateTime, string from, string to)
        {
            var todaysTrips = await GetTripsFromTo(routeId, dateTime, from, to);
            return todaysTrips.OrderBy(x => x.From.DepartureTime).Where(x => x.From.DepartureTime > dateTime.TimeOfDay).Take(3);
        }

        public async Task<IEnumerable<TripFromTo>> GetTripsFromTo(string routeId, DateTime date, string from, string to)
        {
            var dateFormat = date.ToString("yyyy-MM-dd");
            return await GetCachedAsync<TripFromTo>($"trip/{from}/{to}/{routeId}/{dateFormat}");
        }

        public async Task<IEnumerable<TripFromTo>> GetTripsForADay(string routeId, DateTime dateTime, string from, string to, bool showOnlyThree)
        {
            if (showOnlyThree && dateTime.Date == DateTime.Now.Date)
            {
                return await GetNextThreeTrips(routeId, dateTime, from, to);
            }
            else
            {
                return await GetTripsFromTo(routeId, dateTime, from, to);
            }
        }

        public async Task<Tuple<DateTime, DateTime>> GetAvailableDates()
        {
            return await GetEntityFromApiAsync<Tuple<DateTime, DateTime>>(new Uri(baseUrl, "trip/dates"));
        }

        public async Task<IEnumerable<TripFromTo>> GetEligibleTrips(string routeId, DateTime date, string from, string to)
        {
            // eligibledaysavailable = 2 e.g.
            // date = today ? search for today and yesterday
            // date = yesterday ? search for yesterday only
            // date = beforeyesterday ? search nothing
            if (DateTime.Now.Date - date.Date < TimeSpan.FromDays(eligibilityDaysAvailable))
            {
                var dateFormat = date.ToString("yyyy-MM-dd");
                var eligibleTrips = await GetAsync<TripFromTo>($"trip/eligible/{from}/{to}/{routeId}/{dateFormat}");
                return eligibleTrips.Where(x => x.HappenedNotLaterThanDays(eligibilityDaysAvailable));
            }
            return Enumerable.Empty<TripFromTo>();
        }
    }
}