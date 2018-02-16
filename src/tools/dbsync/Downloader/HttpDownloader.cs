using dbsync.Abstractions;
using dbsync.Configuration;
using Serilog;
using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Threading.Tasks;

namespace dbsync.Downloader
{
    class HttpDownloader : IDownloader
    {
        private DownloaderConfiguration config;

        public HttpDownloader(DownloaderConfiguration config)
        {
            this.config = config;
        }

        private string GetTodayFileName() => Path.Combine(config.TargetFolder, DateTime.Now.Date.ToString("MM-dd-yyyy"), "gtfs.zip");

        private void Unzip()
        {
            ZipFile.ExtractToDirectory(GetTodayFileName(), Path.GetDirectoryName(GetTodayFileName()), true);
        }

        public async Task<bool> DownloadArtifactsAsync(bool unzipResults)
        {
            try
            {
                using (var client = new WebClient())
                {
                    client.DownloadProgressChanged += Client_DownloadProgressChanged;
                    Directory.CreateDirectory(Path.GetDirectoryName(GetTodayFileName()));
                    await client.DownloadFileTaskAsync(new Uri(config.DownloadUrl), GetTodayFileName());
                    if (unzipResults)
                    {
                        Unzip();
                    }
                }
            }
            catch (Exception e)
            {
                Log.Error(e, "Error during file downloading");
                return false;
            }
            Log.Information("Files downloaded successfully");
            return true;
        }

        private void Client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            if (e.ProgressPercentage > 0)
                Console.WriteLine($"Downloaded {e.ProgressPercentage}");
        }
    }
}
