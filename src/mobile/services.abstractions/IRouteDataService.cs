using DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace services.abstractions
{
    public interface IRouteDataService
    {
        Task<IEnumerable<Route>> GetRoutesAsync();
    }
}