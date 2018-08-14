using System;
using System.Collections.Generic;
using System.Text;
using DTO;
using System.Linq;

namespace dto.Extensions
{
    public static class StopTimeExtensions
    {
        public static string TripShortId(this StopTime stopTime) => stopTime.TripId.Split('-').Last();
        public static string TripDateString(this StopTime stopTime) => stopTime.TripId.Split('-').First();
        public static DateTime GetDate(this StopTime stopTime) => DateTime.ParseExact(stopTime.TripDateString(), "yyyyMMdd", null);
    }
}
