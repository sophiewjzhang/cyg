using Android.Locations;
using DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace android.Services.Abstractions
{
    public interface IStopDataService
    {
        Task<IEnumerable<Stop>> GetStopsByRoute(string routeId);
        Task<Stop> GetCloserStop(IEnumerable<Stop> stops, Action<Location> callback);
    }
}