using dbsync.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using DTO.Abstractions;
using System.Threading.Tasks;
using dbsync.Configuration;
using System.Data.SqlClient;
using Dapper.Contrib.Extensions;
using System.Linq;
using Dapper;
using System.Data;
using System.Transactions;

namespace dbsync.Writers
{
    public class MsSqlEntityWriter : IDataWriter
    {
        private DbWriterConfiguration configuration;

        public MsSqlEntityWriter(DbWriterConfiguration configuration)
        {
            this.configuration = configuration;
        }

        private string FirstCharToUpper(string input)
        {
            if (String.IsNullOrEmpty(input))
                throw new ArgumentException(input);
            return $"{input.First().ToString().ToUpper()}{input.Substring(1)}";
        }

        private string ConvertToPascalCase(string input)
        {
            return string.Join("", input.Split('_').Select(x => FirstCharToUpper(x)));
        }

        public async Task WriteAsync<T>(DataTable data) where T : class, IEntity
        {
            try
            {
                using (var connection = new SqlConnection(configuration.TargetConnectionString))
                {
                    await connection.OpenAsync();
                    using (var transaction = new TransactionScope(TransactionScopeOption.Required, TimeSpan.FromMinutes(10), TransactionScopeAsyncFlowOption.Enabled))
                    {
                        await connection.DeleteAllAsync<T>();
                        using (var sqlBulkCopy = new SqlBulkCopy(connection))
                        {
                            sqlBulkCopy.BulkCopyTimeout = 10 * 60;
                            foreach (var column in data.Columns)
                            {
                                var dataColumn = (DataColumn)column;
                                sqlBulkCopy.ColumnMappings.Add(dataColumn.ColumnName, ConvertToPascalCase(dataColumn.ColumnName));
                            }
                            // TODO: switch dapper to use singular Stop vs Stops
                            sqlBulkCopy.DestinationTableName = $"{typeof(T).Name}s";
                            await sqlBulkCopy.WriteToServerAsync(data);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw;
            }
        }
    }
}
