using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DTO;

namespace dto.Extensions
{
    public static class TripExtension
    {
        public static string TripShortId(this Trip trip) => trip.TripId.Split('-').Last();
    }
}
