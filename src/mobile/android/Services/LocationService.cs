using android.Services.Abstractions;
using Android.App;
using Android.Locations;
using System.Threading.Tasks;
using Android.OS;
using Android.Runtime;
using Android.Content;
using System;
using android.Exceptions;

namespace android.Services
{
    public class LocationService : Java.Lang.Object, ILocationListener, ILocationService
    {
        private LocationManager locationManager;
        private string locationProvider;
        TaskCompletionSource<Location> tcs;
        Action<Location> callback;


        public LocationService(LocationManager locationManager)
        {
            this.locationManager = locationManager;
            locationProvider = locationManager.GetBestProvider(new Criteria
            {
                Accuracy = Accuracy.Fine
            }, true) ?? string.Empty;
        }

        public Task<Location> GetCurrentLocation(Action<Location> callback)
        {
            tcs = new TaskCompletionSource<Location>();
            this.callback = callback;

            try
            {
                var location = locationManager.GetLastKnownLocation(locationProvider);
                if (location == null)
                {
                    locationManager.RequestSingleUpdate(new Criteria { Accuracy = Accuracy.Fine }, this, null);
                }
                tcs.TrySetResult(location);
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
            tcs.TrySetResult(location);
            callback(location);
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