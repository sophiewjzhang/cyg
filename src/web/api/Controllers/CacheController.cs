using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace api.Controllers
{
    [Route("api/cache")]
    public class CacheController : Controller
    {
        private static string IsInvalidateCacheRequired = null;
        private const string TAG_CACHE = "Хуячим кэш";
        private const string TAG_SETTINGS = "Хуячим сеттинги";
        private const string TAG_CACHE_SETTINGS = "Хуячим кэш и сеттинги";

        [HttpGet("check")]
        public IActionResult CacheCheck() => Ok(IsInvalidateCacheRequired);

        [HttpPost("")]
        public ActionResult InvalidateCacheRequired([FromBody]string tag)
        {
            switch (tag)
            {
                case TAG_CACHE:
                    IsInvalidateCacheRequired = "cache";
                    break;
                case TAG_SETTINGS:
                    IsInvalidateCacheRequired = "settings";
                    break;
                case TAG_CACHE_SETTINGS:
                    IsInvalidateCacheRequired = "cache_settings";
                    break;
                default:
                    IsInvalidateCacheRequired = null;
                    break;
            }
            return Ok();
        }
    }
}
