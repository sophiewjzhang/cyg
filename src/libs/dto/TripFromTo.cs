using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace DTO
{
    public class TripFromTo
    {
        public StopTime From { get; set; }
        public StopTime To { get; set; }
    }
}