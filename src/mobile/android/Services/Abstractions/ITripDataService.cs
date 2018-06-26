using DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace android.Services.Abstractions
{
    public interface ITripDataService
    {
        Task<IEnumerable<TripFromTo>> GetTripsFromTo(string routeId, DateTime date, string from, string to);
        Task<IEnumerable<TripFromTo>> GetTripsFromTo(string routeId, DateTime date, Stop from, Stop to);
        Task<IEnumerable<TripFromTo>> GetNextThreeTrips(string routeId, Stop from, Stop to);
        Task<IEnumerable<TripFromTo>> GetNextThreeTrips(string routeId, string from, string to);
        Task<IEnumerable<TripFromTo>> GetNextThreeTrips(string routeId, DateTime dateTime, string from, string to);
        Task<Tuple<DateTime, DateTime>> GetAvailableDates();
    }
}