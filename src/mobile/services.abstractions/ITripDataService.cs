using DTO;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace services.abstractions
{
    public interface ITripDataService
    {
        Task<IEnumerable<TripFromTo>> GetTripsFromTo(string routeId, DateTime date, string from, string to);
        Task<IEnumerable<TripFromTo>> GetNextThreeTrips(string routeId, DateTime dateTime, string from, string to);
        Task<IEnumerable<TripFromTo>> GetTripsForADay(string routeId, DateTime dateTime, string from, string to, bool showOnlyThree);

        Task<Tuple<DateTime, DateTime>> GetAvailableDates();
        Task<IEnumerable<TripFromTo>> GetEligibleTrips(string routeId, DateTime date, string from,
            string to);
    }
}