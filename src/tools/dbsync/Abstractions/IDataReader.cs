using DTO.Abstractions;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace dbsync.Abstractions
{
    public interface IDataReader
    {
        Task<DataTable> ReadAsync<T>(string fileName) where T : IEntity, new();
    }
}
