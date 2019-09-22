using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DCC.FraudDection;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Configuration;

namespace DCC.SignalR.Demo.Pages
{
    public class IndexModel : PageModel
    {
        private readonly IConfiguration _configuration;
        private readonly MockDetection _detector;


        public IndexModel(IConfiguration configuration)
        {
            _configuration = configuration;
            _detector = new MockDetection(_configuration);
        }
        public IList<FraudUIAlerts> Alerts { get; private set; } = new List<FraudUIAlerts>();


        //[BindProperty(SupportsGet = true)]
        [BindProperty]
        public DateTime StartDateTimePicker { get; set; }
        //[BindProperty(SupportsGet = true)]
        [BindProperty]
        public DateTime EndDateTimePicker { get; set; }

        public void OnGet(DateTime startDateTime, DateTime endDateTime)
        {
            Alerts = _detector.GetAlerts();
            if (Alerts.Any())
            {
                StartDateTimePicker = startDateTime == DateTime.MinValue ? Alerts.Min(x => x.Timestamp) : startDateTime;
                if (endDateTime.ToString() == DateTime.MaxValue.ToString() ||
                    (DateTime.Compare(endDateTime, DateTime.MinValue) == 0))
                    EndDateTimePicker = Alerts.Max(x => x.Timestamp);
                else EndDateTimePicker = endDateTime;

                if (EndDateTimePicker < StartDateTimePicker)
                {
                    EndDateTimePicker = Alerts.Max(x => x.Timestamp);
                }

            }
            else
            {
                StartDateTimePicker = DateTime.Now;
                EndDateTimePicker = DateTime.UtcNow;
            }

            //todo filter by timestamp if present
            if ((StartDateTimePicker > DateTime.MinValue) && (EndDateTimePicker < DateTime.MaxValue))
            {
                IEnumerable<FraudUIAlerts> AlertsFiltered = Alerts.Where(a => (a.Timestamp >= StartDateTimePicker) && (a.Timestamp <= EndDateTimePicker));
                Alerts = AlertsFiltered.ToList();
            }
        }

        public IActionResult OnPost()
        {
            DateTime dateTime;
            var parsed = DateTime.TryParse(Request.Form["startdate"], out dateTime);

            StartDateTimePicker = parsed ? dateTime : DateTime.MinValue;
            parsed = DateTime.TryParse(Request.Form["enddate"], out dateTime);
            EndDateTimePicker = parsed ? dateTime : DateTime.MaxValue;

            return RedirectToPage($"./Index", new { startDateTime = StartDateTimePicker.ToString(), endDateTime = EndDateTimePicker.ToString() });
        }
    }
}
