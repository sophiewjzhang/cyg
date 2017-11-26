using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using dbsync.Configuration;

namespace dbsync
{
    class Program
    {
        private static IConfigurationRoot Configuration { get; set; }

        static void Main(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            var downloaderConfig = configuration.GetSection("downloader").Get<DownloaderConfiguration>();
            var fileSystemReaderConfig = configuration.GetSection("fileSystemReader").Get<FileSystemReaderConfiguration>();
            var writerConfig = configuration.GetSection("writer").Get<DbWriterConfiguration>();

            Console.ReadKey();
        }

        static void ConfigChanged(object o)
        {
            Console.WriteLine("Config changed");
            //((IChangeToken)o).RegisterChangeCallback(ConfigChanged, o);
        }
    }
}