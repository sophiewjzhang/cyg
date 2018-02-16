using DTO;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace android.Services.Abstractions
{
    public interface IStopDataService
    {
        Task<IEnumerable<Stop>> GetStopsByRoute(string routeId);
    }
}