using Android.App;
using Android.OS;

namespace android
{
    [Activity(Label = "android")]
    public class Trips : Activity
    {
        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.Trips);
        }
    }
}