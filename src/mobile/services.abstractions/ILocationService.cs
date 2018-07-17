using models;
using System;
using System.Threading.Tasks;

namespace services.abstractions
{
    public interface ILocationService
    {
        Task<Location> GetCurrentLocation(Action<Location> callback);
        double GetDistance(double lat1, double lon1, double lat2, double lon2);
        bool IsGpsAvailable();
    }
}