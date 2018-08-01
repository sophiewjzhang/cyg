using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using dal;
using dto.Extensions;
using DTO;
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
                    var hoursRange = 2;
                    var hoursBackShift = 1;
                    if (args.Length > 0)
                    {
                        hoursBackShift = int.Parse(args[0]);
                        if (args.Length > 1)
                        {
                            hoursRange = int.Parse(args[1]);
                        }
                    }

                    var stopwatch = new Stopwatch();
                    stopwatch.Start();
                    var configuration = new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                        .Build();

                    var tripRepository = new TripRepository(configuration.GetValue<string>("TargetConnectionString"), configuration.GetValue<string>("EligibilityApiUrl"));

                    var endDatetime = DateTime.Now.AddHours(-1 * hoursBackShift);
                    var startDatetime = endDatetime.AddHours(-1 * hoursRange);
                    var tripsToProcess = await tripRepository.GetAllTripsFromToByDate(startDatetime, endDatetime);

                    var toProcess = tripsToProcess as StopTime[] ?? tripsToProcess.ToArray();
                    Console.WriteLine($"{toProcess.Count()} found between {startDatetime} and {endDatetime}");

                    foreach (var trip in toProcess)
                    {
                        var isEligible = await tripRepository.IsTripEligible(trip);
                        Console.WriteLine($"Trip {trip.TripId} {trip.StopHeadsign} {trip.GetDate()} is { (isEligible ? "eligible" : "NOT eligible") } for return");
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
