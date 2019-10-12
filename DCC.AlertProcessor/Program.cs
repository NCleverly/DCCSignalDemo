using DCC.FraudDection;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Threading.Tasks;

namespace DCC.AlertProcessor
{
    class Program
    {
        static void Main(string[] args)
        {
            MainTask().Wait();
        }

        private static async Task MainTask()
        {
            try
            {
                var environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

                var builder = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                    .AddJsonFile($"appsettings.{environmentName}.json", false, true)
                    .AddEnvironmentVariables();

                IConfigurationRoot configuration = builder.Build();

                MockDetection detector = new MockDetection(configuration);

                var requests = detector.GetAlerts();
                foreach (var request in requests)
                {
                    await detector.SendAlert(request);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}