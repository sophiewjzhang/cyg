using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace dbsync.Abstractions
{
    public interface IDownloader
    {
        Task<bool> DownloadArtifacts(bool unzipResults);
    }
}
