using dbsync.Abstractions;
using System;
using System.Collections.Generic;
using System.Text;
using DTO.Abstractions;
using System.Threading.Tasks;
using dbsync.Configuration;

namespace dbsync.Writers
{
    public class MsSqlEntityWriter : IDataWriter
    {
        private DbWriterConfiguration configuration;

        public MsSqlEntityWriter(DbWriterConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public Task Write<T>(T data) where T : IEntity
        {
            throw new NotImplementedException();
        }
    }
}
