using DTO.Abstractions;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Threading.Tasks;

namespace dbsync.Abstractions
{
    public interface IDataWriter
    {
        Task WriteAsync<T>(DataTable data) where T : class, IEntity;
    }
}
