using System;

namespace DCC.FraudDection
{
    public class FraudUIAlerts
    {
        public long FraudAlertID { get; set; }

        public Guid? AlertUxID { get; set; }

        public string MemberID { get; set; }

        public string AgentAssigned { get; set; }

        public bool FraudTransaction { get; set; }

        public DateTime CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }

        public string ETan { get; set; }

        public string RulesTripped { get; set; }

        public DateTime Timestamp { get; set; }

    }
}
