using DTO;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dal.Abstractions
{
    public interface ITripRepository
    {
        Task<IEnumerable<Trip>> GetTripsByRoute(string routeId);
        Task<IEnumerable<Trip>> GetTripsByRouteAndDate(string routeId, DateTime date);
        Task<IEnumerable<StopTime>> GetAllTripsFromToByDate(DateTime startDateTime, DateTime endDateTime);
        Task<IEnumerable<TripFromTo>> GetTripsFromToByRouteIdAndDate(string fromId, string toId, string routeId, DateTime date);
        Task<IEnumerable<TripFromTo>> GetNextThreeTrips(string fromId, string toId, string routeId);
        Task<Trip> GetTrip(string tripId);
        Task<Tuple<DateTime, DateTime>> GetAvailableDates();
        Task<bool> IsTripEligible(StopTime stopTime);
        Task<int> UpdateTripEligibility(StopTime endStopTime, bool isEligible);
    }
}
