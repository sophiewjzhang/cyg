using DTO;
using models;
using services.abstractions;
using services.abstractions.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace services
{
    public class StopDataService : BaseDataService, IStopDataService
    {
        private ILocationService locationService;

        public StopDataService(ICacheService cacheService, ILocationService locationService, string baseUrl, int apiTimeoutInSeconds) : base(cacheService, baseUrl, apiTimeoutInSeconds)
        {
            this.locationService = locationService;
        }

        public async Task<IEnumerable<Stop>> GetStopsByRoute(string routeId)
        {
            return await GetCachedAsync<Stop>($"stop/{routeId}");
        }

        public async Task<Stop> GetCloserStop(IEnumerable<Stop> stops, Action<Location> callback)
        {
            var location = await locationService.GetCurrentLocation(callback);
            if (location == null)
            {
                throw new LocationIsNotAvailableException();
            }
            return stops.Aggregate((curMin, x) => (curMin == null || DistanceToStop(location, x) < DistanceToStop(location, curMin) ? x : curMin));
        }

        private double DistanceToStop(Location location, Stop stop)
        {
            return locationService.GetDistance(location.Lat, location.Lon, stop.StopLat, stop.StopLon);
        }
    }
}