using android.extensions;
using android.Views;
using Android.App;
using Android.Content;
using Android.OS;
using Android.Views;
using Android.Widget;
using Autofac;
using models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using services.abstractions.Exceptions;
using services.abstractions;
using android.Services;

namespace android
{
    [Activity(Label = "Settings")]
    public class MainActivity : StopSpinnerActivity
    {
        private IList<SpinnerItem> routes = null;
        private IRouteDataService routeDataService;
        private IUserSettingsService userSettingsService;
        private Spinner spinnerRoute;
        private RadioButton showOnlyThreeCheckbox;
        private RadioButton showAllCheckbox;
        private CheckBox locationSwitchCheckBox;
        private Button searchButton;
        private TextView textViewException;
        private TextView textViewEnableLocation;
        private bool firstRun = true;
        private float initialY;
        private ILocationService locationService;

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            routeDataService = App.Container.Resolve<IRouteDataService>();
            userSettingsService = App.Container.Resolve<IUserSettingsService>();
            locationService = App.Container.Resolve<ILocationService>();

            SetContentView(Resource.Layout.Main);

            base.InitControls(Save, false, useIds: true);

            settings = await userSettingsService.LoadUserSettings();

            textViewException = FindViewById<TextView>(Resource.Id.textViewNoInternet);
            spinnerRoute = FindViewById<Spinner>(Resource.Id.spinnerRoute);
            spinnerRoute.ItemSelected += async (s, e) => { await RouteSpinner_ItemSelected(s, e); };

            showOnlyThreeCheckbox = FindViewById<RadioButton>(Resource.Id.radioButton1);
            showAllCheckbox = FindViewById<RadioButton>(Resource.Id.radioButton2);
            // TODO: change with value of radioGroup
            showOnlyThreeCheckbox.Checked = settings?.ShowOnlyThreeTrips ?? true;
            showOnlyThreeCheckbox.CheckedChange += async (s1, e1) => { await Save(); };
            showAllCheckbox.Checked = !showOnlyThreeCheckbox.Checked;
            showAllCheckbox.CheckedChange += async (s1, e1) => { await Save(); };

            locationSwitchCheckBox = FindViewById<CheckBox>(Resource.Id.checkBox1);
            locationSwitchCheckBox.Checked = locationService.IsGpsAvailable() && (settings?.SwapDirectionBasedOnLocation ?? true);
            locationSwitchCheckBox.Enabled = locationService.IsGpsAvailable();
            locationSwitchCheckBox.CheckedChange += async (s1, e1) => { await Save(); };

            textViewEnableLocation = FindViewById<TextView>(Resource.Id.textViewEnableLocation);
            textViewEnableLocation.Visibility = locationService.IsGpsAvailable() ? ViewStates.Gone : ViewStates.Visible;

            searchButton = FindViewById<Button>(Resource.Id.button1);
            searchButton.Click += async (t, e1) => { await SearchButton_Click(t, e1); };

            var extra = Intent.GetStringExtra("action");
            if (settings != null && extra != "edit")
            {
                var intent = new Intent(this, typeof(TripsActivity));
                StartActivity(intent);
            }
            if (settings != null)
            {
                searchButton.Text = "Back";
            }

            await LoadRoutes();
        }

        protected async Task LoadRoutes()
        {
            try
            {
                loader.StartAnimation(animation);
                routes = (await routeDataService.GetRoutesAsync()).Select(x => new SpinnerItem(x.RouteId, x.RouteLongName)).ToList();
                loader.ClearAnimation();
                this.AddRouteSpinnerItemsWithSelectedValue(spinnerRoute, routes, settings?.RouteId, viewId: Resource.Drawable.IdSpinner, defaultValueResourceId: Resource.String.select_route);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task RouteSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            HideError<GenericNoConnectionException>();
            if (e.Position == 0 && settings?.RouteId == null)
            {
                return;
            }

            var newValue = routes.FirstOrDefault(x => x.Value == spinnerRoute.SelectedItem.ToString())?.Id;
            if (settings?.RouteId == null)
            {
                this.AddRouteSpinnerItemsWithSelectedValue(spinnerRoute, routes, newValue, Resource.Drawable.IdSpinner);
            }
            if (settings == null)
            {
                settings = new UserSettings();
            }
            if (newValue != settings?.RouteId || firstRun)
            {
                firstRun = false;
                var from = settings.RouteId == newValue ? settings.From : null;
                var to = settings.RouteId == newValue ? settings.To : null;
                settings.RouteId = newValue;
                await InitStopSpinners(newValue, from, to);
            }
        }

        private async Task SearchButton_Click(object sender, System.EventArgs e)
        {
            await Save();

            var intent = new Intent(this, typeof(TripsActivity));
            StartActivity(intent);
        }

        private async Task Save()
        {
            if (string.IsNullOrWhiteSpace(settings?.RouteId)
                || settings?.To == null
                || settings?.From == null)
            {
                return;
            }

            userSettingsService.SaveUserSettings(new UserSettings
            {
                RouteId = settings.RouteId,
                From = settings.From,
                To = settings.To,
                ShowOnlyThreeTrips = showOnlyThreeCheckbox.Checked,
                SwapDirectionBasedOnLocation = locationSwitchCheckBox.Checked
            });
        }

        protected override void ShowError(Exception e)
        {
            textViewException.Text = e.Message;
            textViewException.Visibility = ViewStates.Visible;
        }

        protected override void HideError<T>()
        {
            textViewException.Visibility = ViewStates.Gone;
            textViewException.Text = "";
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Move:
                    var diff = e.GetY() - initialY;
                    if (diff > 1000)
                    {
                        CacheService.InvalidateCache();
                    }
                    break;
                case MotionEventActions.Down:
                    initialY = e.GetY();
                    break;
            }
            return base.DispatchTouchEvent(e);
        }
    }
}

