using DTO;
using services.abstractions;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace services
{
    public class RouteDataService : BaseDataService, IRouteDataService
    {
        public RouteDataService(ICacheService cacheService, string baseUrl, int apiTimeoutInSeconds) : base(cacheService, baseUrl, apiTimeoutInSeconds)
        {
        }

        public async Task<IEnumerable<Route>> GetRoutesAsync()
        {
            return await GetCachedAsync<Route>("route");
        }
    }
}