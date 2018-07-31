using DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dal.Abstractions
{
    public interface IStopTimeRepository
    {
        Task<IEnumerable<StopTime>> GetStopTimesByTrip(string tripId);
        Task<IEnumerable<StopTime>> GetStopTimesByStop(string stopId);
    }
}
