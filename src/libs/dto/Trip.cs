using System;

namespace DTO
{
    public class Trip
    {
        public DateTime DepartureTime { get; set; }
        public DateTime ArrivalTime { get; set; }
        public bool IsDelayed { get; set; }
        public TimeSpan DelayedFor { get; set; }
        public bool IsQualifiedForClaim { get; set; }

        //route_id,service_id,trip_id,trip_headsign,
        //trip_short_name,direction_id,block_id,shape_id,wheelchair_accessible,bikes_allowed,route_variant
        public long Id { get; set; }
        public string RouteId { get; set; }
        public string ServiceId { get; set; }
        /// <summary>
        /// Trip Id in GTFS
        /// </summary>
        public string DisplayId { get; set; }
        public string HeadSign { get; set; }
        public string ShortName { get; set; }
        public int DirectionId { get; set; }
        public string BlockId { get; set; }
        public string ShapeId { get; set; }
        public int WheelChairAccessible { get; set; }
        public int BikesAllowed { get; set; }
        public string RouteVariant { get; set; }
    }
}