using DCC.FraudDection;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace DCC.FraudDetection.Repos
{
    public class SignalRRepo
    {
        private readonly HubConnection _connection;

        public SignalRRepo(IConfiguration config)
        {
            _connection = new HubConnectionBuilder()
                    .WithUrl(config["SignalR:Url"] + config["SignalR:HubName"])
                    .Build();
            _connection.Closed += async (error) =>
            {
                await Task.Delay(new Random().Next(0, 5) * 1000);
                await _connection.StartAsync();
            };
        }

        public async Task<bool> SendAlert(FraudUIAlerts alert)
        {
            try
            {
                await _connection.StartAsync();
                await _connection.InvokeAsync("SendMessage", alert);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return true;
        }
    }
}
