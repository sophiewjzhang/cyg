using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using dal;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace eligibilitysync
{
    class Program
    {
        static void Main(string[] args)
        {
            Task.Run(async () =>
            {
                try
                {
                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();

                    var yesterday = DateTime.Now.Date.AddDays(-1);
                    var tripRepository = new TripRepository(configuration.GetValue<string>("TargetConnectionString"), configuration.GetValue<string>("EligibilityApiUrl"));

                    var endDatetime = DateTime.Now;
                    var startDatetime = endDatetime.AddDays(-2);
                    var yesterdayTrips = await tripRepository.GetAllTripsFromToByDate(startDatetime, endDatetime);

                    foreach (var trip in yesterdayTrips)
                    {
                        var isEligible = await tripRepository.IsTripEligible(trip, yesterday);
                        Console.WriteLine($"Trip {trip.TripId} {trip.StopHeadsign} is { (isEligible ? "eligible" : "NOT eligible") } for return");
                        await tripRepository.UpdateTripEligibility(trip, isEligible);
                    }
                    Console.WriteLine($"elapsed {stopwatch.ElapsedMilliseconds}");

                    stopwatch.Stop();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            });
            Console.ReadLine();
        }
    }
}
