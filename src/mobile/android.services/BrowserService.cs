using System;
using System.Collections;
using System.Threading.Tasks;
using Android.Content;
using DTO;
using services.abstractions;
using System.Collections.Generic;
using dto.Extensions;

namespace android.services
{
    public class BrowserService : IBrowserService
    {
        private static Context context;
        private string serviceGuaranteeUrl;

        public BrowserService(string serviceGuaranteeUrl)
        {
            this.serviceGuaranteeUrl = serviceGuaranteeUrl;
        }

        public static void Init(Context currentActivity)
        {
            context = currentActivity;
        }

        public void OpenServiceGuaranteePage(TripFromTo trip, DateTime dateTime, string from, string to)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }
            
            var url = $"{ serviceGuaranteeUrl }?TripNo={trip.GetTripShortId()}&DPTStn={from}&DPTStnID={trip.From.StopId}&ARVStn={to}&ARVStnID={trip.To.StopId}&Date={ dateTime:MM/dd/yyyy}&Time={trip.From.DepartureTime.Hours}:{trip.From.DepartureTime.Minutes}";
            var uri = Android.Net.Uri.Parse(url);

            var browserIntent = new Intent(Intent.ActionView, uri);
            context.StartActivity(browserIntent);
        }
    }
}