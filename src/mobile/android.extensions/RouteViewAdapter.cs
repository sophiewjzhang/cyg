using Android.Content;
using Android.Graphics;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace android.extensions
{
    public class RouteViewAdapter : BaseViewAdapter
    {
        private int parentSpinnerWidth = 0;

        public RouteViewAdapter(Context ctx, int resourceId, IList<SpinnerItem> objects) : base(ctx, resourceId, objects)
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
                        parentSpinnerWidth = parentSpinner.Width;
                    }
                }
                else if (position == temporarySelectedItemPosition)
                {
                    textView.SetBackgroundColor(new Color(0xc8, 0xf3, 0xba));
                }
                textView.SetWidth(parentSpinnerWidth);
                textView.SetPadding(40, 0, 0, 0);
                textView.SetTextColor(Color.Black);
                textView.SetTextSize(Android.Util.ComplexUnitType.Dip, 35);
                textView.SetElegantTextHeight(true);
                return textView;
            }
            return dropDownView;
        }

        protected override View GetCustomView(int position, View convertView, ViewGroup parent)
        {
            var inflater = (LayoutInflater)(Context.GetSystemService(Context.LayoutInflaterService));
            var mainText = inflater.Inflate(spinnerResourceId, parent, false) as TextView;
            mainText.TextSize = 35f;
            mainText.SetTextColor(Color.Black);
            mainText.Text = GetItem(position).Value;

            return mainText;
        }
    }
}