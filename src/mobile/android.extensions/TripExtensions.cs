using DTO;
using System;

namespace android.extensions
{
    public static class TripExtensions
    {
        public static string GetTripTimeText(this TripFromTo trip)
        {
            var tripTimeSpan = GetTripTime(trip.From.DepartureTime, trip.To.ArrivalTime);
            var hours = Math.Abs(tripTimeSpan.Hours);
            var minutes = Math.Abs(tripTimeSpan.Minutes);
            if (hours > 0)
            {
                return $"{hours} h {minutes} min";
            }
            return $"{minutes} min";
        }

        private static TimeSpan GetTripTime(TimeSpan departure, TimeSpan arrival)
        {
            // + 24 to cover case when departure 23:** and arrival 00:**
            if (departure > arrival)
            {
                arrival = new TimeSpan(arrival.Hours + 24, arrival.Minutes, arrival.Seconds);
            }
            return arrival - departure;
        }
    }
}