using DTO.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class StopTime : IEntity
    {
        public string TripId { get; set; }
        public TimeSpan ArrivalTime { get; set; }
        public TimeSpan DepartureTime { get; set; }
        public string StopId { get; set; }
        public int StopSequence { get; set; }
        public int PickupType { get; set; }
        public int DropOffType { get; set; }
        public string StopHeadsign { get; set; }
    }
}
