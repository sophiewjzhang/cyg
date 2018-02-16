using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Collections;
using dal.Abstractions;

namespace api.Controllers
{
    [Produces("application/json")]
    [Route("api/Route")]
    public class RouteController : Controller
    {
        private IRouteRepository routeRepository;

        public RouteController(IRouteRepository routeRepository)
        {
            this.routeRepository = routeRepository;
        }

        public async Task<IActionResult> Index()
        {
            var result = await routeRepository.GetTrainRoutes();
            return Ok(result);
        }
    }
}