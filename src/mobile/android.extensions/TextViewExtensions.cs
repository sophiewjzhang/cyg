using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Widget;
using static Android.Views.ViewGroup;

namespace android.extensions
{
    public static class TextViewExtensions
    {
        public static TextView GetTextViewTripListStyle(this Context context, string text, Color background, float dencity, LayoutParams layoutParams = null, bool isEligible = false, string id = null, float weight = 0.3f)
        {
            var textView = new TextView(context)
            {
                Text = text,
                LayoutParameters = layoutParams ?? new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.MatchParent, weight)
            };

            textView.SetTextColor(isEligible ? Color.OrangeRed : Color.White);
            textView.SetBackgroundColor(background);
            textView.SetPadding(DpToPx(dencity, 20), DpToPx(dencity, 5), DpToPx(dencity, 5), DpToPx(dencity, 5));
            textView.SetTextSize(Android.Util.ComplexUnitType.Dip, 17);
            if (!string.IsNullOrWhiteSpace(id))
            {
                textView.Id = id.GetHashCode();
            }

            return textView;
        }

        public static void ApplyStopStyle(this TextView textView, Color? background = null, string text = null)
        {
            if (text != null)
            {
                textView.Text = text;
            }
            if (background.HasValue)
            {
                textView.SetBackgroundColor(background.Value);
            }
            textView.SetTextSize(Android.Util.ComplexUnitType.Dip, 17);
            textView.SetTextColor(Color.White);
        }

        public static void ApplyRouteStyle(this TextView textView, Color? background = null, string text = null)
        {
            if (text != null)
            {
                textView.Text = text;
            }
            if (background.HasValue)
            {
                textView.SetBackgroundColor(background.Value);
            }
            textView.SetTextSize(Android.Util.ComplexUnitType.Dip, 35);
            textView.SetTextColor(Color.Black);
            textView.SetElegantTextHeight(true);
        }

        private static int DpToPx(float dencity, int dp)
        {
            return (int)(dp * dencity + 0.5f);
        }
    }
}