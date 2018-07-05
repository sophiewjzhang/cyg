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
using System.Diagnostics;

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
                    var sw = new Stopwatch();
                    sw.Start();
                    Console.WriteLine($"Is download success: {await downloader.DownloadArtifactsAsync(true)} in {sw.ElapsedMilliseconds} ms");

                    sw.Restart();
                    var stops = await reader.ReadAsync<Stop>("stops.txt");
                    Console.WriteLine($"Read {stops.Rows.Count} objects from stops.txt in {sw.ElapsedMilliseconds} ms");
                    await dbWriter.WriteAsync<Stop>(stops);
                    Console.WriteLine($"Saved {stops.Rows.Count} stops to databaset in {sw.ElapsedMilliseconds} ms");

                    sw.Restart();
                    var trips = await reader.ReadAsync<Trip>("trips.txt");
                    Console.WriteLine($"Read {trips.Rows.Count} objects from trips.txt in {sw.ElapsedMilliseconds} ms");
                    await dbWriter.WriteAsync<Trip>(trips);
                    Console.WriteLine($"Saved {trips.Rows.Count} trips to databaset in {sw.ElapsedMilliseconds} ms");

                    sw.Restart();
                    var stopTimes = await reader.ReadAsync<StopTime>("stop_times.txt");
                    ////var rows = stopTimes.Select("stop_id not in ('YO','WR','WH','WE','USBT','UN','UI','ST','SR','SC','RU','RO','RI','PO','PIN','PA','OS','OR','OL','OA','NE','MR','MP','MO','ML','MK','MJ','MI','ME','MA','LS','LO','LI','LA','KP','KI','KE','KC','HA','GU','GO','GL','GE','EX','ET','ER','EG','EA','DW','DI','DA','CO','CL','CE','BU','BR','BO','BL','BE','BD','BA','AU','AP','AL','AJ','AG','AD','AC','SCTH','NI')");

                    ////for (int i = rows.Length - 1; i >= 0; i--)
                    ////{
                    ////    rows[i].Delete();
                    ////}
                    Console.WriteLine($"Read {stopTimes.Rows.Count} objects from stop_times.txt in {sw.ElapsedMilliseconds} ms");
                    await dbWriter.WriteAsync<StopTime>(stopTimes);
                    Console.WriteLine($"Saved {stopTimes.Rows.Count} stopTimes to database in {sw.ElapsedMilliseconds} ms");

                    sw.Restart();
                    var routes = await reader.ReadAsync<Route>("routes.txt");
                    Console.WriteLine($"Read {routes.Rows.Count} objects from routes.txt in {sw.ElapsedMilliseconds} ms");
                    await dbWriter.WriteAsync<Route>(routes);
                    Console.WriteLine($"Saved {routes.Rows.Count} routes to database in {sw.ElapsedMilliseconds} ms");
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