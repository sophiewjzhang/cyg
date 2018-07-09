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
        private int spinnerFromResourceId;
        private int rightBackgroundResourceId;
        private int leftBackgroundResourceId;

        public StopViewAdapter(Context ctx, int resourceId, IList<SpinnerItem> objects, int spinnerFromResourceId, int rightBackgroundResourceId, int leftBackgroundResourceId) : base(ctx, resourceId, objects)
        {
            this.spinnerFromResourceId = spinnerFromResourceId;
            this.rightBackgroundResourceId = rightBackgroundResourceId;
            this.leftBackgroundResourceId = leftBackgroundResourceId;
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
                    return dropDownView;
                }

                textView.ApplyStopStyle(position == temporarySelectedItemPosition ? new Color(0x2a, 0x5c, 0x12) : new Color(0x37, 0x42, 0x32));
                textView.SetPadding(20, 20, 20, 20);
                return textView;
            }
            return dropDownView;
        }

        protected override View GetCustomView(int position, View convertView, ViewGroup parent)
        {
            var inflater = (LayoutInflater)(Context.GetSystemService(Context.LayoutInflaterService));
            var mainText = inflater.Inflate(spinnerResourceId, parent, false) as TextView;

            // test to minify size needed for station name
            mainText.ApplyStopStyle(text: GetItem(position).Value.Replace(" GO", ""));
            mainText.Gravity = ((Spinner)parent).Gravity;
            var relativeLayout = ((RelativeLayout)parent.Parent);
            if (mainText.Text.Length > 17)
            {
                relativeLayout.SetBackgroundColor(new Color(0x37, 0x42, 0x32));
            }
            else
            {
                // TODO: do something with this shit
                if (parent.Id == spinnerFromResourceId)
                {
                    relativeLayout.SetBackgroundResource(leftBackgroundResourceId);
                }
                else
                {
                    relativeLayout.SetBackgroundResource(rightBackgroundResourceId);
                }
            }

            return mainText;
        }
    }
}