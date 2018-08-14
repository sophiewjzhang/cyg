using DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dal.Abstractions
{
    public interface IStopRepository
    {
        Task<IEnumerable<Stop>> GetTrainStops();
        Task<Stop> GetStop(string stopId);
        Task<IEnumerable<Stop>> GetTrainStopsByTrip(string tripId);
        Task<IEnumerable<Stop>> GetStopsByRoute(string routeId);
        Task<IEnumerable<Stop>> GetStopsByRouteAndDate(string routeId, DateTime date);
    }
}
