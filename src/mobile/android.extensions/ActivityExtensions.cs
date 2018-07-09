using Android.Content;
using Android.Widget;
using Java.Util;
using System.Collections.Generic;
using System.Linq;
using utils;

namespace android.extensions
{
    public static class ActivityExtensions
    {
        private static IList<SpinnerItem> GetItemsArrayWithDefaultValue(Context context, IList<SpinnerItem> items, int defaultValueResourceId)
        {
            return Enumerable.Concat(new SpinnerItem[] { new SpinnerItem(string.Empty, context.Resources.GetString(defaultValueResourceId)) }, items).ToList();
        }

        public static void AddStopSpinnerItemsWithSelectedValue(this Context context, Spinner spinner, IList<SpinnerItem> items, string selectedId, int spinnerFromResourceId, int rightBackgroundResourceId, int leftBackgroundResourceId, int viewId = Android.Resource.Layout.SimpleSpinnerItem, int defaultValueResourceId = -1)
        {
            if (selectedId == null && defaultValueResourceId >= 0)
            {
                items = GetItemsArrayWithDefaultValue(context, items, defaultValueResourceId);
            }
            spinner.Adapter = new StopViewAdapter(context, viewId, items, spinnerFromResourceId, rightBackgroundResourceId, leftBackgroundResourceId);
            // weird bug with moving spinner 20 px if no PostDelayed
            if (selectedId != null)
            {
                spinner.PostDelayed(() => spinner.SetSelection(items.IndexOf(x => x.Id == selectedId), false), 1);
            }
        }

        public static void AddRouteSpinnerItemsWithSelectedValue(this Context context, Spinner spinner, IList<SpinnerItem> items, string selectedId, int viewId = Android.Resource.Layout.SimpleSpinnerItem, int defaultValueResourceId = -1)
        {
            if (selectedId == null && defaultValueResourceId >= 0)
            {
                items = GetItemsArrayWithDefaultValue(context, items, defaultValueResourceId);
            }
            spinner.Adapter = new RouteViewAdapter(context, viewId, items);
            if (selectedId != null)
            {
                spinner.PostDelayed(() => spinner.SetSelection(items.IndexOf(x => x.Id == selectedId), false), 1);
            }
        }
    }
}
