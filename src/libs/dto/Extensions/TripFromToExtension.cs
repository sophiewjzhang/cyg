using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTO;

namespace dto.Extensions
{
    public static class TripFromToExtension
    {
        public static string GetTripShortId(this TripFromTo trip) => trip.From.TripId.Split('-').Last();
        public static bool GetEligibility(this TripFromTo trip) => trip.To.IsEligible ?? false;
        public static DateTime GetDate(this TripFromTo trip) => trip.From.GetDate();
        public static bool HappenedNotLaterThanDays(this TripFromTo trip, int days) => DateTime.Now.Date - trip.GetDate() < TimeSpan.FromDays(days);
    }
}
