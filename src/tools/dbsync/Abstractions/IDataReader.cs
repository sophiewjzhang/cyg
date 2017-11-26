using DTO.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dbsync.Abstractions
{
    public interface IDataReader
    {
        Task<IEnumerable<T>> Read<T>() where T : IEntity;
    }
}
