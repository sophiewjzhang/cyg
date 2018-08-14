using DTO.Abstractions;
using System;
using System.Linq;
using Newtonsoft.Json;

namespace DTO
{
    public class Trip : IEntity
    {
        public string RouteId { get; set; }
        public string ServiceId { get; set; }
        public string TripId { get; set; }
        public string TripHeadsign { get; set; }
        public string TripShortName { get; set; }
        public int DirectionId { get; set; }
        public string BlockId { get; set; }
        public string ShapeId { get; set; }
        public int WheelchairAccessible { get; set; }
        public string BikesAllowed { get; set; }
        public string RouteVariant { get; set; }
    }
}