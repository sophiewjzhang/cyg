using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using dbsync.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using dbsync.Downloader;
using System.Threading.Tasks;
using dbsync.Readers;
using DTO;
using System.Linq;
using dbsync.Writers;

namespace dbsync
{
    class Program
    {
        private static IConfigurationRoot Configuration { get; set; }

        static void SetupConfig(IServiceCollection sc)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            sc.AddSingleton(configuration.GetSection("downloader").Get<DownloaderConfiguration>());
            sc.AddSingleton(configuration.GetSection("fileSystemReader").Get<FileSystemReaderConfiguration>());
            sc.AddSingleton(configuration.GetSection("writer").Get<DbWriterConfiguration>());
        }

        static void SetupLogging(IServiceCollection sc)
        {
            sc.AddLogging();
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Elasticsearch()
                .CreateLogger();
        }

        static void Main(string[] args)
        {
            var sc = new ServiceCollection();

            SetupConfig(sc);
            SetupLogging(sc);

            var sp = sc.BuildServiceProvider();

            var downloader = new HttpDownloader(sp.GetService<DownloaderConfiguration>());
            var reader = new FileReader(sp.GetService<FileSystemReaderConfiguration>());
            var dbWriter = new MsSqlEntityWriter(sp.GetService<DbWriterConfiguration>());
            Task.Run(async () => {
                try
                {
                    //Console.WriteLine($"Is download success: {await downloader.DownloadArtifactsAsync(true)}");

                    //var stops = await reader.ReadAsync<Stop>("stops.txt");
                    //Log.Information($"Read {stops.Rows.Count} objects from stops.txt");
                    //await dbWriter.WriteAsync<Stop>(stops);
                    //Log.Information($"Saved {stops.Rows.Count} stops to database");

                    //var trips = await reader.ReadAsync<Trip>("trips.txt");
                    //Log.Information($"Read {trips.Rows.Count} objects from trips.txt");
                    //await dbWriter.WriteAsync<Trip>(trips);
                    //Log.Information($"Saved {trips.Rows.Count} trips to database");

                    var stopTimes = await reader.ReadAsync<StopTime>("stop_times.txt");
                    Log.Information($"Read {stopTimes.Rows.Count} objects from stop_times.txt");
                    await dbWriter.WriteAsync<StopTime>(stopTimes);
                    Log.Information($"Saved {stopTimes.Rows.Count} stopTimes to database");

                    var routes = await reader.ReadAsync<Route>("routes.txt");
                    Log.Information($"Read {routes.Rows.Count} objects from routes.txt");
                    await dbWriter.WriteAsync<Route>(routes);
                    Log.Information($"Saved {routes.Rows.Count} routes to database");
                }
                catch (Exception e)
                {
                    Log.Error(e, "Unhandled exception occured");
                }
            });
            Console.ReadKey();
        }
    }
};