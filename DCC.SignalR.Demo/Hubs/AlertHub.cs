using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using DCC;
using DCC.FraudDection;
using DCC.FraudDetection.Models;
using DCC.FraudDetection.Services;

namespace DCC.SignalR.Demo.Hubs
{
    public class AlertHub: Hub
    {
        private readonly IConfiguration _configuration;
        private readonly MockDetection _detector;

        public AlertHub(IConfiguration configuration, IRavenDbAccess<FraudUIAlerts> raven)
        {
            _configuration = configuration;
            _detector = new MockDetection(_configuration, raven);
        }
        public async Task SendMessage(FraudUIAlerts alert)
        {
            await Clients.All.SendAsync("ReceiveMessage", alert.MemberID, alert.RulesTripped, alert.ETan,
                alert.Timestamp, alert.AlertUxID);
        }

        public async Task RemoveMessage(string id, bool isFraud)
        {
            var currentUser = Context.User.Identity.Name;

            _detector.SetFraudStatus(Guid.Parse(id), currentUser, isFraud);

            await Clients.All.SendAsync("DeleteMessage", id);
        }
    }
}
