using dal.Abstractions;
using System;
using DTO;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.IO;
using Dapper;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using dto;
using dto.Extensions;
using Newtonsoft.Json;

namespace dal
{
    public class TripRepository : ITripRepository
    {
        private string connectionString;
        private string eligibilityUrl;

        public TripRepository(string connectionString, string eligibilityUrl = null)
        {
            this.connectionString = connectionString;
            this.eligibilityUrl = eligibilityUrl;
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
where serviceid = @date
and st1.stopid = @fromId
and st2.stopid = @toId
and st1.StopSequence < st2.StopSequence
and t.tripid like '%-' + @routeId + '-%'
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

        public async Task<IEnumerable<StopTime>> GetAllTripsFromToByDate(DateTime startDateTime, DateTime endDateTime)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<StopTime>(@"select distinct st2.* from trips t
inner join stoptimes st1 on st1.tripid = t.tripid
inner join stoptimes st2 on st2.tripid = t.tripid
inner join routes r on r.routeid = t.RouteId
where CAST(convert(datetime, ServiceId) as datetime) + CAST(st2.ArrivalTime as datetime) >= @startDateTime
and CAST(convert(datetime, ServiceId) as datetime) + CAST(st2.ArrivalTime as datetime) <= @endDateTime 
and st1.StopSequence < st2.StopSequence
and r.routetype = 2", new
                    {
                        startDateTime = startDateTime,
                        endDateTime = endDateTime
                    }, commandTimeout: 300);
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

        public async Task<bool> IsTripEligible(StopTime endStopTime)
        {
            if (string.IsNullOrEmpty(eligibilityUrl))
            {
                throw new ArgumentNullException(nameof(eligibilityUrl));
            }

            var stringContent = JsonConvert.SerializeObject(new
            {
                dateString = endStopTime.GetDate().ToString("MMddyyyy"),
                arrivalstationCode = endStopTime.StopId,
                tripNumber = endStopTime.TripShortId(),
                lang = "en",
            });
            var client = (HttpWebRequest) WebRequest.Create(new Uri(eligibilityUrl));
            client.ContentType = "application/json";
            client.Method = "POST";
            using (var streamWriter = new StreamWriter(client.GetRequestStream()))
            {
                streamWriter.Write(stringContent);
                streamWriter.Flush();
                streamWriter.Close();
            }

            var httpResponse = (HttpWebResponse)(await client.GetResponseAsync());

            if (httpResponse == null) return false;

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var eligibleString = streamReader.ReadToEnd();
                var result = JsonConvert.DeserializeObject<IsEligibleResult>(eligibleString);
                return result.CheckEligibleResult.ResultType == 1;
            }

        }

        public async Task<int> UpdateTripEligibility(StopTime endStopTime, bool isEligible)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();

                return await connection.ExecuteAsync("update StopTimes set IsEligible = @isEligible where TripId = @tripId and StopId = @stopId ",
                    new {isEligible = isEligible, tripId = endStopTime.TripId, stopId = endStopTime.StopId});
            }
        }
    }
}
