namespace DTO
{
    public class Stop
    {
        public Stop()
        {

        }
        public Stop(string name, double latitude, double longitude)
        {
            Name = name;
            Latitude = latitude;
            Longitude = longitude;
        }
        public long Id { get; set; }
        public string DisplayId { get; set; }
        public string Name { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public int ZoneId { get; set; }
        public string StopUrl { get; set; }
        public int LocationType { get; set; }
        public Stop ParentStation { get; set; }
        public int WheelChairBoarding { get; set; }
    }
}
