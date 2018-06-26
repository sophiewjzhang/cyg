using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace android.extensions
{
    public class StopViewAdapter : BaseViewAdapter
    {
        private static Dictionary<string, Drawable> backgrounds = new Dictionary<string, Drawable>();

        public StopViewAdapter(Context ctx, int resourceId, IList<SpinnerItem> objects) : base(ctx, resourceId, objects)
        {
        }

        public override View GetDropDownView(int position, View convertView, ViewGroup parent)
        {
            var dropDownView = base.GetDropDownView(position, convertView, parent);

            if (dropDownView is TextView textView)
            {
                // ugly hack to get current selected position
                if (parent is Spinner parentSpinner)
                {
                    if (parentSpinner.SelectedItemPosition == position)
                    {
                        temporarySelectedItemPosition = position;
                    }
                }
                else if (position == temporarySelectedItemPosition)
                {
                    textView.SetBackgroundColor(new Color(0x2a, 0x5c, 0x12));
                }
                else
                {
                    textView.SetBackgroundColor(new Color(0x37, 0x42, 0x32));
                }
                textView.SetTextSize(Android.Util.ComplexUnitType.Dip, 17);
                textView.SetTextColor(Color.White);
                textView.SetPadding(20, 20, 20, 20);
                return textView;
            }
            return dropDownView;
        }

        protected override View GetCustomView(int position, View convertView, ViewGroup parent)
        {
            var inflater = (LayoutInflater)(Context.GetSystemService(Context.LayoutInflaterService));
            var mainText = inflater.Inflate(spinnerResourceId, parent, false) as TextView;
            mainText.SetTextSize(Android.Util.ComplexUnitType.Dip, 17);
            mainText.SetTextColor(Color.White);

            // test to minify size needed for station name
            mainText.Text = GetItem(position).Value.Replace(" GO", "");
            mainText.Gravity = ((Spinner)parent).Gravity;
            var relativeLayout = ((RelativeLayout)parent.Parent);
            if (mainText.Text.Length > 17)
            {
                relativeLayout.SetBackgroundColor(new Color(0x37, 0x42, 0x32));
            }
            else
            {
                // TODO: do something with this shit
                if (parent.Id == 2131230865)
                {
                    relativeLayout.SetBackgroundResource(2130837780);
                }
                else
                {
                    relativeLayout.SetBackgroundResource(2130837781);
                }
            }

            return mainText;
        }
    }
}