using dal.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using DTO;
using System.Threading.Tasks;
using System.Data.SqlClient;
using Dapper;

namespace dal
{
    public class RouteRepository : IRouteRepository
    {
        private string connectionString;

        public RouteRepository(string connectionString)
        {
            this.connectionString = connectionString;
        }

        public async Task<Route> GetRoute(string routeId)
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryFirstAsync<Route>(@"select * from Routes where RouteId like '%-' + @routeId", routeId);
            }
        }

        public async Task<IEnumerable<Route>> GetRoutes()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<Route>(@"select * from Routes");
            }
        }

        public async Task<IEnumerable<Route>> GetTrainRoutes()
        {
            using (var connection = new SqlConnection(connectionString))
            {
                await connection.OpenAsync();
                return await connection.QueryAsync<Route>(@"select distinct
                    RIGHT([RouteId], LEN([RouteId])-CHARINDEX('-', [RouteId])) RouteId
                      ,[AgencyId]
                      ,[RouteShortName]
                      ,[RouteLongName]
                      ,[RouteType]
                      ,[RouteColor]
                      ,[RouteTextColor] 
                    from Routes where RouteType = 2 order by RouteId");
            }
        }
    }
}
