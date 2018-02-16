using dal.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/[controller]")]
    public class TripController : Controller
    {
        private ITripRepository tripRepository;

        public TripController(ITripRepository tripRepository)
        {
            this.tripRepository = tripRepository;
        }

        [HttpGet("{fromId}/{toId}/{routeId}/{date}")]
        public async Task<IActionResult> FromTo(string fromId, string toId, string routeId, DateTime date)
        {
            var result = await tripRepository.GetTripsFromToByRouteIdAndDate(fromId, toId, routeId, date);
            return Ok(result);
        }

        [HttpGet("{fromId}/{toId}/{routeId}")]
        public async Task<IActionResult> NextThreeTrips(string fromId, string toId, string routeId)
        {
            var result = await tripRepository.GetNextThreeTrips(fromId, toId, routeId);
            return Ok(result);
        }
    }
}
