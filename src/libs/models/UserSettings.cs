using System;

namespace models
{
    public class UserSettings
    {
        public string RouteId { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public bool SwapDirectionBasedOnLocation { get; set; }
        public bool ShowOnlyThreeTrips { get; set; }
    }
}
