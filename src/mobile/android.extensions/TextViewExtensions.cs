using Android.Content;
using Android.Content.Res;
using Android.Graphics;
using Android.Widget;
using static Android.Views.ViewGroup;

namespace android.extensions
{
    public static class TextViewExtensions
    {
        public static TextView GetTextViewTripListStyle(this Context context, string text, Color background, float dencity, LayoutParams layoutParams = null)
        {
            var textView = new TextView(context)
            {
                Text = text,
                LayoutParameters = layoutParams ?? new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.MatchParent, 0.3f)
            };
            textView.SetTextColor(Color.White);
            textView.SetBackgroundColor(background);
            textView.SetPadding(DpToPx(dencity, 20), DpToPx(dencity, 5), DpToPx(dencity, 5), DpToPx(dencity, 5));
            textView.SetTextSize(Android.Util.ComplexUnitType.Dip, 17);
            return textView;
        }

        public static int DpToPx(float dencity, int dp)
        {
            return (int)(dp * dencity + 0.5f);
        }

    }
}