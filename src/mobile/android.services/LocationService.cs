using Android.Locations;
using Android.OS;
using Android.Runtime;
using services.abstractions;
using services.abstractions.Exceptions;
using System;
using System.Threading.Tasks;

namespace android.services
{
    public class LocationService : Java.Lang.Object, ILocationListener, ILocationService
    {
        private readonly LocationManager locationManager;
        TaskCompletionSource<models.Location> tcs;
        Action<models.Location> callback;

        public LocationService(LocationManager locationManager)
        {
            this.locationManager = locationManager;
        }

        private static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddMilliseconds(timestamp).ToLocalTime();
        }

        public async Task<models.Location> GetCurrentLocation(Action<models.Location> callback)
        {
            this.callback = callback;

            try
            {
                if (!locationManager.IsProviderEnabled("gps"))
                {
                    throw new LocationIsNotAvailableException();
                }
                var location = locationManager.GetLastKnownLocation("gps");
                if (location == null || ConvertFromUnixTimestamp(location.Time) < DateTime.Now.AddHours(-1))
                {
                    locationManager.RequestSingleUpdate("gps", this, null);
                }

                if (location == null)
                    return null;
                    
                return new models.Location
                {
                    Lat = location.Latitude,
                    Lon = location.Longitude
                };
            }
            catch (Exception)
            {
                throw new LocationIsNotAvailableException();
            }
        }

        public double GetDistance(double lat1, double lon1, double lat2, double lon2)
        {
            var distances = new float[3];
            Location.DistanceBetween(lat1, lon1, lat2, lon2, distances);
            return distances[0];
        }

        public bool IsGpsAvailable()
        {
            return locationManager.IsProviderEnabled("gps");
        }

        public void OnLocationChanged(Location location)
        {
            if (ConvertFromUnixTimestamp(location.Time) >= DateTime.Now.AddHours(-1))
            {
                var locationModel = new models.Location
                {
                    Lat = location.Latitude,
                    Lon = location.Longitude
                };
                callback(locationModel);
            }
        }

        public void OnProviderDisabled(string provider)
        {
        }

        public void OnProviderEnabled(string provider)
        {
        }

        public void OnStatusChanged(string provider, [GeneratedEnum] Availability status, Bundle extras)
        {
        }
    }
}