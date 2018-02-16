using android.Services.Abstractions;
using DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace android.Services
{
    public class StopDataService : BaseDataService, IStopDataService
    {
        private const string AllStopsCacheKey = "Stops";

        public StopDataService(ICacheService cacheService, string baseUrl) : base(cacheService, baseUrl)
        {
        }

        public async Task<IEnumerable<Stop>> GetStopsByRoute(string routeId)
        {
            return await GetAsync<Stop>($"{AllStopsCacheKey}:{routeId}", $"stop/{routeId}");
        }
    }
}