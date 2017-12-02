using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using dbsync.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using dbsync.Downloader;
using System.Threading.Tasks;

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
            Task.Run(async () => {
                Console.WriteLine($"Is download success: {await downloader.DownloadArtifactsAsync(true)}");
            });
            Console.ReadKey();
        }
    }
}