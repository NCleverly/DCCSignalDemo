using DCC.FraudDection;
using DCC.FraudDetection.Models;
using DCC.FraudDetection.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

                IConfiguration configuration = builder.Build();
                var serviceProvider = new ServiceCollection()
                    .AddSingleton<IKeyVaultService, KeyVaultService>()
                    .AddScoped<IDocumentStoreHolder, DocumentStoreHolder>().AddScoped<IRavenDbAccess<FraudUIAlerts>, FraudDataAccess>()
                    .BuildServiceProvider();
                var raven = serviceProvider.GetService<IRavenDbAccess<FraudUIAlerts>>();

                MockDetection detector = new MockDetection(configuration, raven);

                var requests = detector.GetMockData();
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