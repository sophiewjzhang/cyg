using Android.Locations;
using DTO;
using System;
using System.Threading.Tasks;

namespace android.Services.Abstractions
{
    public interface ILocationService
    {
        Task<Location> GetCurrentLocation(Action<Location> callback);
        double GetDistance(double lat1, double lon1, double lat2, double lon2);
    }
}