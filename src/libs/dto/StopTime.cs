using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DTO
{
    public class StopTime
    {
//trip_id,       arrival_time,departure_time,stop_id,stop_sequence,pickup_type,drop_off_type
//6200-Sun-40040,06:45:00,    06:45:00,      00350,  15,           0,          0
        public long Id { get; set; }
        public Trip Trip { get; set; }
        public DateTime ArrivalTime { get; set; }
        public DateTime DepartureTime { get; set; }
        public Stop Stop { get; set; }
        public int StopSequence { get; set; }
        public int PickupType { get; set; }
        public int DropOffType { get; set; }
    }
}
