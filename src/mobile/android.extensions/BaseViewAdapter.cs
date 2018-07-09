using Android.Content;
using Android.Views;
using Android.Widget;
using System.Collections.Generic;

namespace android.extensions
{
    public abstract class BaseViewAdapter : ArrayAdapter<SpinnerItem>
    {
        protected int spinnerResourceId;
        protected int temporarySelectedItemPosition;
        protected string gravity;

        public BaseViewAdapter(Context ctx, int resourceId, IList<SpinnerItem> objects) : base(ctx, resourceId, objects)
        {
            spinnerResourceId = resourceId;
        }

        public IEnumerable<SpinnerItem> Values
        {
            get
            {
                for (var i = 0; i < Count; i++)
                {
                    yield return GetItem(i);
                }
            }
        }

        public override View GetView(int position, View cnvtView, ViewGroup prnt)
        {
            return GetCustomView(position, cnvtView, prnt);
        }

        protected abstract View GetCustomView(int position, View convertView, ViewGroup parent);
    }
}