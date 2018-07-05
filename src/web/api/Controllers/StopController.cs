using dal.Abstractions;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/[controller]")]
    public class StopController : Controller
    {
        private IStopRepository stopRepository;

        public StopController(IStopRepository stopRepository)
        {
            this.stopRepository = stopRepository;
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
            var result = await stopRepository.GetStopsByRoute(routeId);
            return Ok(result);
        }
    }
}
