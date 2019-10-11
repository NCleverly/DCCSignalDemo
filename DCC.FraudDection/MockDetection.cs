using DCC.FraudDetection.Models;
using DCC.FraudDetection.Repos;
using DCC.FraudDetection.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DCC.FraudDection
{
    public class MockDetection
    {
        private readonly SignalRRepo hubRepo;
        IRavenDbAccess<FraudUIAlerts> _raven;
        public MockDetection(IConfiguration _configuration, IRavenDbAccess<FraudUIAlerts> raven)
        {
            hubRepo = new SignalRRepo(_configuration);

            _raven = raven;
        }

        public void SetupMockData()
        {
            List<FraudUIAlerts> items = GetMockData();
            foreach (var item in items)
            {
                item.id = item.AlertUxID.ToString();
                _raven.AddItem(item);
            }
        }

        public List<FraudUIAlerts> GetMockData()
        {
            return new List<FraudUIAlerts> {
                new FraudUIAlerts
                {
                    id = Guid.NewGuid().ToString(),
                    AlertUxID = Guid.NewGuid(),
                    CreatedDate = DateTime.UtcNow.AddMinutes(-20),
                    ETan = "7984765348964",
                    FraudAlertID = 870345,
                    MemberID = "MEM46546546SDFS",
                    ModifiedDate = DateTime.UtcNow.AddMinutes(-20),
                    RulesTripped = "Too many different credit cards in One Hour Too many transactions",
                    Timestamp = DateTime.UtcNow.AddMinutes(-20)
                },
                new FraudUIAlerts
                {
                    id = Guid.NewGuid().ToString(),
                    AlertUxID = Guid.NewGuid(),
                    CreatedDate = DateTime.UtcNow.AddMinutes(-10),
                    ETan = "4654654",
                    FraudAlertID = 870345,
                    MemberID = "MEM46546546SDFS",
                    ModifiedDate = DateTime.UtcNow.AddMinutes(-10),
                    RulesTripped = "Totally daily transaction amount limit reached Too many transactions",
                    Timestamp = DateTime.UtcNow.AddMinutes(-12)
                },
                new FraudUIAlerts
                {

                    AlertUxID = Guid.NewGuid(),
                    CreatedDate = DateTime.UtcNow.AddMinutes(-10),
                    ETan = "46541365213",
                    FraudAlertID = 870345,
                    MemberID = "MEM46546546SDFS",
                    ModifiedDate = DateTime.UtcNow.AddMinutes(-10),
                    RulesTripped = "Too many different credit cards in One Hour Too many transactions",
                    Timestamp = DateTime.UtcNow.AddMinutes(-10)
                },
                new FraudUIAlerts
                {
                    id = Guid.NewGuid().ToString(),
                    AlertUxID = Guid.NewGuid(),
                    CreatedDate = DateTime.UtcNow.AddMinutes(-5),
                    ETan = "sdahdshlaslads",
                    FraudAlertID = 870345,
                    MemberID = "MEM46546546SDFS",
                    ModifiedDate = DateTime.UtcNow.AddMinutes(-5),
                    RulesTripped = "Too many different credit cards in One Hour Too many transactions",
                    Timestamp = DateTime.UtcNow.AddMinutes(-5)
                }
            };
        }

        public IList<FraudUIAlerts> GetAlerts()
        {
            var items =_raven.GetAllItems();
            return items;
        }


        public async Task<bool> SendAlert(FraudUIAlerts request) => await hubRepo.SendAlert(request);

        //Todo: send the remove alert
        public void SetFraudStatus(Guid uxId, string currentUser, bool isFraud)
        {
            FraudUIAlerts alert = new FraudUIAlerts { AgentAssigned = currentUser, AlertUxID = uxId, id=uxId.ToString(), FraudTransaction = isFraud};
            if (isFraud)
            {
                Debug.WriteLine($"alert id:{uxId} was marked as fraud by {currentUser}");

                _raven.Update(alert);
            }
            else
            {
                Debug.WriteLine($"alert id:{uxId} was marked as NOT fraud by {currentUser}");
            }
        }

    }
}
