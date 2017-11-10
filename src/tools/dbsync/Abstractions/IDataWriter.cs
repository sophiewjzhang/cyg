using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dbsync.Abstractions
{
    public interface IDataWriter
    {
        Task Write<T>(T data);
    }
}
