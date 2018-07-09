using DTO.Abstractions;

namespace DTO
{
    public class Stop : IEntity
    {
        public string StopId { get; set; }
        public string StopName { get; set; }
        public double StopLat { get; set; }
        public double StopLon { get; set; }
        public string ZoneId { get; set; }
        public string StopUrl { get; set; }
        public int LocationType { get; set; }
        public string ParentStation { get; set; }
        public int WheelChairBoarding { get; set; }
    }
}
