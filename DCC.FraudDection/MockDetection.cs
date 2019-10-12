using DCC.FraudDetection.Repos;
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
        public MockDetection(IConfiguration _configuration)
        {
            hubRepo = new SignalRRepo(_configuration);
        }

        //todo:send the fake Data
        public IList<FraudUIAlerts> GetAlerts()
        {
            var items = new List<FraudUIAlerts> {
                new FraudUIAlerts
                {
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
            return items;
        }


        public async Task<bool> SendAlert(FraudUIAlerts request) => await hubRepo.SendAlert(request);

        //Todo: send the remove alert
        public void SetFraudStatus(Guid uxId, string currentUser, bool isFraud)
        {
            if (isFraud)
            {
                Debug.WriteLine($"alert id:{uxId} was marked as fraud by {currentUser}");
            }
            else
            {
                Debug.WriteLine($"alert id:{uxId} was marked as NOT fraud by {currentUser}");
            }
        }

    }
}
