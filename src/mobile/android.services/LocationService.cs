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
        private LocationManager locationManager;
        private string locationProvider;
        TaskCompletionSource<models.Location> tcs;
        Action<models.Location> callback;

        public LocationService(LocationManager locationManager)
        {
            this.locationManager = locationManager;
            locationProvider = locationManager.GetBestProvider(new Criteria
            {
                Accuracy = Accuracy.Fine
            }, true) ?? string.Empty;
        }

        private static DateTime ConvertFromUnixTimestamp(double timestamp)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            return origin.AddMilliseconds(timestamp).ToLocalTime();
        }

        public Task<models.Location> GetCurrentLocation(Action<models.Location> callback)
        {
            tcs = new TaskCompletionSource<models.Location>();
            this.callback = callback;

            try
            {
                var location = locationManager.GetLastKnownLocation(locationProvider);
                if (location == null || ConvertFromUnixTimestamp(location.Time) < DateTime.Now.AddHours(-1))
                {
                    locationManager.RequestSingleUpdate(new Criteria { Accuracy = Accuracy.Fine }, this, null);
                }

                if (location != null)
                    tcs.TrySetResult(new models.Location
                    {
                        Lat = location.Latitude,
                        Lon = location.Longitude
                    });
                return tcs.Task;
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

        public void OnLocationChanged(Location location)
        {
            var locationModel = new models.Location
            {
                Lat = location.Latitude,
                Lon = location.Longitude
            };
            tcs.TrySetResult(locationModel);
            callback(locationModel);
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