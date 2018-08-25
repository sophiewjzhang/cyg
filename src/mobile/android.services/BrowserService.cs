using System;
using System.Collections;
using System.Threading.Tasks;
using Android.Content;
using DTO;
using services.abstractions;
using System.Collections.Generic;
using dto.Extensions;
using Android.Widget;
using Android.Views;
using System.Globalization;

namespace android.services
{
    public class BrowserService : IBrowserService
    {
        private static Context context;
        private string serviceGuaranteeUrl;
        private ClipboardManager clipboardManager;

        public BrowserService(string serviceGuaranteeUrl, ClipboardManager clipboardManager)
        {
            this.serviceGuaranteeUrl = serviceGuaranteeUrl;
            this.clipboardManager = clipboardManager;
        }

        public static void Init(Context currentActivity)
        {
            context = currentActivity;
        }

        public void OpenServiceGuaranteePage(TripFromTo trip, DateTime dateTime, string from, string to, string prestoCardNumber)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            clipboardManager.PrimaryClip = ClipData.NewPlainText("prestoNumber", prestoCardNumber);
            var toast = Toast.MakeText(context,
                "Your Presto Number has been copied to clipboard. You will be redirected to Metrolinx website to submit claim.",
                ToastLength.Long);
            toast.SetGravity(GravityFlags.Center, 0, 0);
            toast.Show();

            var url = $"{ serviceGuaranteeUrl }?TripNo={trip.GetTripShortId()}&DPTStn={from}&DPTStnID={trip.From.StopId}&ARVStn={to}&ARVStnID={trip.To.StopId}&Date={ dateTime.ToString("MM/dd/yyyy", CultureInfo.InvariantCulture) }&Time={trip.From.DepartureTime.Hours.ToString().PadLeft(2, '0')}:{trip.From.DepartureTime.Minutes.ToString().PadLeft(2, '0')}";
            var uri = Android.Net.Uri.Parse(url);

            var browserIntent = new Intent(Intent.ActionView, uri);
            context.StartActivity(browserIntent);
        }
    }
}