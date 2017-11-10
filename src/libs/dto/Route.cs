using System;

namespace DTO
{
    public class Route
    {
        public long Id { get; set; }
        /// <summary>
        /// Route id in GTFS
        /// </summary>
        public string DisplayId { get; set; }
        public string AgencyId { get; set; }
        public string ShortName { get; set; }
        public string LongName { get; set; }
        public int Type { get; set; }
        public string Color { get; set; }
        public string TextColor { get; set; }
//        route_id,agency_id,route_short_name,route_long_name,route_type,route_color,route_text_color
//258-ST,  GO,       ST,              Stouffville,    2,         794500,     FFFFFF
    }
}
