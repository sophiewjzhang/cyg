using dal.Abstractions;
using DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/[controller]")]
    public class StopController : Controller
    {
        private IStopRepository stopRepository;
        private IMemoryCache cache;

        public StopController(IStopRepository stopRepository, IMemoryCache cache)
        {
            this.stopRepository = stopRepository;
            this.cache = cache;
        }

        [ResponseCache()]
        public async Task<IActionResult> Index()
        {
            var result = await stopRepository.GetTrainStops();
            return Ok(result);
        }

        [HttpGet("{routeId}")]
        public async Task<IActionResult> ByRoute(string routeId)
        {
            if (cache.TryGetValue(routeId, out IEnumerable<Stop> stops))
            {
                return Ok(stops);
            }

            stops = await stopRepository.GetStopsByRoute(routeId);
            cache.Set(routeId, stops, TimeSpan.FromDays(90));

            return Ok(stops);
        }
    }
}
