using DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dal.Abstractions
{
    public interface IRouteRepository
    {
        Task<IEnumerable<Route>> GetRoutes();
        Task<IEnumerable<Route>> GetTrainRoutes();
        Task<Route> GetRoute(string routeId);
    }
}
