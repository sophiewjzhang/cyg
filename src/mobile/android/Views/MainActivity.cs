using Android.App;
using Android.OS;
using DTO;
using System.Collections.Generic;
using android.Services.Abstractions;
using Autofac;
using System.Net.Http;
using System.Threading.Tasks;
using Android.Widget;
using System.Linq;
using System;

namespace android
{
    [Activity(Label = "android", MainLauncher = true)]
    public class MainActivity : Activity
    {
        private IEnumerable<Route> routes = null;
        private IEnumerable<Stop> stops = null;
        private IRouteDataService routeDataService;
        private IStopDataService stopDataService;
        private ITripDataService tripDataService;
        private Spinner routeSpinner;
        private Spinner fromSpinner;
        private Spinner toSpinner;
        private EditText date;
        private Button searchButton;

        private string selectedRouteId = null;
        private Stop selectedFrom = null;
        private Stop selectedTo = null;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            routeDataService = App.Container.Resolve<IRouteDataService>();
            stopDataService = App.Container.Resolve<IStopDataService>();
            tripDataService = App.Container.Resolve<ITripDataService>();

            SetContentView(Resource.Layout.Main);

            routeSpinner = FindViewById<Spinner>(Resource.Id.spinner0);
            routeSpinner.ItemSelected += async (s, e) => { await RouteSpinner_ItemSelected(s, e); };

            fromSpinner = FindViewById<Spinner>(Resource.Id.spinner1);
            fromSpinner.ItemSelected += async (s1, e1) => { await FromSpinner_ItemSelected(s1, e1); };

            toSpinner = FindViewById<Spinner>(Resource.Id.spinner2);
            toSpinner.ItemSelected += ToSpinner_ItemSelected;

            date = FindViewById<EditText>(Resource.Id.editText1);

            searchButton = FindViewById<Button>(Resource.Id.button1);
            searchButton.Click += async (t, e1) => { await SearchButton_Click(t, e1); };

            await LoadRoutes();
        }

        protected async Task LoadRoutes()
        {
            routes = await routeDataService.GetRoutesAsync();
            routeSpinner.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, routes.Select(x => x.RouteLongName).ToArray());
        }

        private async Task RouteSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            selectedRouteId = routes.First(x => x.RouteLongName == routeSpinner.GetItemAtPosition(e.Position).ToString()).RouteId;

            stops = await stopDataService.GetStopsByRoute(selectedRouteId);
            fromSpinner.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, stops.Select(x => x.StopName).ToArray());
        }

        private async Task FromSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            selectedFrom = stops.FirstOrDefault(x => x.StopName == fromSpinner.SelectedItem.ToString());

            var spinner2Stops = stops.Where(x => x.StopName != fromSpinner.GetItemAtPosition(e.Position).ToString());
            toSpinner.Adapter = new ArrayAdapter<string>(this, Android.Resource.Layout.SimpleSpinnerDropDownItem, spinner2Stops.Select(x => x.StopName).ToArray());
        }

        private void ToSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            selectedTo = stops.FirstOrDefault(x => x.StopName == toSpinner.SelectedItem.ToString());
        }

        private async Task SearchButton_Click(object sender, System.EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(selectedRouteId)
                || selectedTo == null
                || selectedFrom == null
                || string.IsNullOrWhiteSpace(date?.Text))
            {
                return;
            }

            var to = stops.FirstOrDefault(x => x.StopName == toSpinner.SelectedItem.ToString());
            var trips = await tripDataService.GetTripsFromTo(selectedRouteId, DateTime.Parse(date.Text), selectedFrom, selectedTo);
            StartActivity(typeof(Trips));
        }
    }
}

