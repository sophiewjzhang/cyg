﻿using dal.Abstractions;
using System;
using DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;
using System.Linq;

namespace dal
{
    public class TripRepository : ITripRepository
    {
        private string connectionString;

        public TripRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<IEnumerable<TripFromTo>> GetNextThreeTrips(string fromId, string toId, string routeId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<StopTime, StopTime, TripFromTo>(@"select TOP 3 st1.*, st2.* from trips t
inner join stoptimes st1 on st1.tripid = t.tripid
inner join stoptimes st2 on st2.tripid = t.tripid
inner join routes r on r.routeid = t.RouteId
where serviceid = @date
and st1.stopid = @fromId
and st2.stopid = @toId
and st1.StopSequence < st2.StopSequence
and r.routetype = 2
and r.routeid like '%-' + @routeId
and st1.DepartureTime > @time
order by st1.ArrivalTime ASC", (from, to) => new TripFromTo
                {
                    From = from,
                    To = to
                }, new
                {
                    routeId = routeId,
                    date = DateTime.Now.ToString("yyyyMMdd"),
                    time = DateTime.Now.ToString("HH:mm:ss"),
                    fromId = fromId,
                    toId = toId
                },
                splitOn: "TripId");
            }
        }

        public Task<Trip> GetTrip(string tripId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Trip>> GetTripsByRoute(string routeId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Trip>> GetTripsByRouteAndDate(string routeId, DateTime date)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TripFromTo>> GetTripsFromToByRouteIdAndDate(string fromId, string toId, string routeId, DateTime date)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<StopTime, StopTime, TripFromTo>(@"select st1.*, st2.* from trips t
inner join stoptimes st1 on st1.tripid = t.tripid
inner join stoptimes st2 on st2.tripid = t.tripid
inner join routes r on r.routeid = t.RouteId
where serviceid = @date
and st1.stopid = @fromId
and st2.stopid = @toId
and st1.StopSequence < st2.StopSequence
and r.routetype = 2
and r.routeid like '%-' + @routeId
order by st1.ArrivalTime ASC", (from, to) => new TripFromTo
                {
                    From = from,
                    To = to
                }, new
                {
                    routeId = routeId,
                    date = date.ToString("yyyyMMdd"),
                    fromId = fromId,
                    toId = toId
                },
                splitOn: "TripId");
            }
        }

        public async Task<Tuple<DateTime, DateTime>> GetAvailableDates()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return (await connection.QueryAsync<DateTime, DateTime, Tuple<DateTime, DateTime>>("select min(convert(datetime, ServiceId)), max(convert(datetime, ServiceId)) from Trips", Tuple.Create, splitOn: "*")).First();
            }
        }
    }
}
