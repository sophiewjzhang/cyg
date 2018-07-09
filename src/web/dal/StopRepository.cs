using Dapper;
using dal.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using DTO;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace dal
{
    public class StopRepository : IStopRepository
    {
        private string connectionString;

        public StopRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<Stop> GetStop(string stopId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstAsync<Stop>("select * from Stops where StopId = @StopId ", stopId);
            }
        }

        public async Task<IEnumerable<Stop>> GetTrainStops()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<Stop>(@"select distinct s.* from Routes r 
  inner join Trips t on t.RouteId = r.RouteId
  inner join StopTimes st on st.TripId = t.TripId
  inner join Stops s on s.StopId = st.StopId
  where routetype = 2");
            }
        }

        public async Task<IEnumerable<Stop>> GetStopsByRoute(string routeId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<Stop>(@"select distinct s.* from Trips t
  inner join StopTimes st on st.TripId = t.TripId
  inner join Stops s on s.StopId = st.StopId
  inner join Routes r on r.RouteId = t.RouteId
  where r.RouteId in (select top 1 RouteId from Routes where RouteId like '%-' + @routeId)", new { routeId = routeId });
            }
        }

        public async Task<IEnumerable<Stop>> GetTrainStopsByTrip(string tripId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<Stop>(@"select distinct s.* from Trips t
  inner join StopTimes st on st.TripId = t.TripId
  inner join Stops s on s.StopId = st.StopId
  where TripId = @tripId order by StopSequence", new { tripId = tripId });
            }
        }

        public async Task<IEnumerable<Stop>> GetStopsByRouteAndDate(string routeId, DateTime date)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<Stop>(@"select distinct s.* from Trips t
  inner join StopTimes st on st.TripId = t.TripId
  inner join Stops s on s.StopId = st.StopId
  where RouteId like '%-' + @routeId 
    and ServiceId = @date", new { routeId = routeId, date = date.ToString("yyyyMMdd") });
            }
        }
    }
}
