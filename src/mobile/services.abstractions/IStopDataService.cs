using DTO;
using models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace services.abstractions
{
    public interface IStopDataService
    {
        Task<IEnumerable<Stop>> GetStopsByRoute(string routeId);
        Task<Stop> GetCloserStop(IEnumerable<Stop> stops, Action<Location> callback, Location location = null);
    }
}