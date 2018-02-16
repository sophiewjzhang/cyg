using DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace android.Services.Abstractions
{
    public interface ITripDataService
    {
        Task<IEnumerable<TripFromTo>> GetTripsFromTo(string routeId, DateTime date, Stop from, Stop to);
        Task<IEnumerable<TripFromTo>> GetNextThreeTrips(string routeId, Stop from, Stop to);
    }
}