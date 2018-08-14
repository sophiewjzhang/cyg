using android.extensions;
using android.Views;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.OS;
using Android.Views;
using Android.Widget;
using Autofac;
using DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using android.services;
using dto.Extensions;
using static Android.App.DatePickerDialog;
using services.abstractions.Exceptions;
using services.abstractions;

namespace android
{
    [Activity(Label = "Trips")]
    public class TripsActivity : StopSpinnerActivity, IOnDateSetListener
    {
        #region services

        private IRouteDataService routeDataService;
        private ITripDataService tripDataService;
        private IBrowserService browserService;
        private IUserSettingsService userSettingsService;

        #endregion

        #region controls

        private ImageButton buttonSettings;
        private Spinner spinnerRoute;
        private TextView textViewDate;
        private TextView textViewDate1;
        private TextView textViewException;
        private TextView textViewNoLocation;
        private LinearLayout layoutMessageTextView;
        private LinearLayout messageTextViewYesterday;
        private LinearLayout messageTextViewNoTrips;
        private DateTime dateSelected;
        private LinearLayout parentLayout;
        private ViewFlipper flipper;
        private TextView currentDateTextView;
        private ImageButton buttonLeft;
        private ImageButton buttonRight;

        #endregion

        #region private members

        private float initialX;
        private bool isFreshMove = false;
        private Tuple<DateTime, DateTime> availableDates;
        private IList<SpinnerItem> routes;
        private Dictionary<string, string> selectedFromCache = new Dictionary<string, string>();
        private Dictionary<string, string> selectedToCache = new Dictionary<string, string>();

        #endregion

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Trips);

            ResolveDependencies();
            InitControls();
            await LoadSettings();
        }

        private void ResolveDependencies()
        {
            userSettingsService = App.Container.Resolve<IUserSettingsService>();
            routeDataService = App.Container.Resolve<IRouteDataService>();
            tripDataService = App.Container.Resolve<ITripDataService>();
            browserService = App.Container.Resolve<IBrowserService>();
            BrowserService.Init(this);
        }

        private async void InitControls()
        {
            base.InitControls(ShowTrips, true);

            flipper = FindViewById<ViewFlipper>(Resource.Id.flipper);
            buttonSettings = FindViewById<ImageButton>(Resource.Id.buttonSettings);
            currentDateTextView = textViewDate = FindViewById<TextView>(Resource.Id.textViewDate);
            textViewDate1 = FindViewById<TextView>(Resource.Id.textViewDate1);
            parentLayout = FindViewById<LinearLayout>(Resource.Id.layoutTrips);
            spinnerRoute = FindViewById<Spinner>(Resource.Id.spinnerRoute);
            textViewException = FindViewById<TextView>(Resource.Id.textViewNoInternet);
            textViewNoLocation = FindViewById<TextView>(Resource.Id.textViewNoLocation);
            layoutMessageTextView = FindViewById<LinearLayout>(Resource.Id.layoutMessageTextView);
            messageTextViewYesterday = FindViewById<LinearLayout>(Resource.Id.messageTextViewYesterday);
            messageTextViewNoTrips = FindViewById<LinearLayout>(Resource.Id.messageTextViewNoTrips);
            buttonLeft = FindViewById<ImageButton>(Resource.Id.buttonLeft);
            buttonRight = FindViewById<ImageButton>(Resource.Id.buttonRight);

            buttonLeft.Click += (s, e) => PreviousDate();
            buttonRight.Click += (s, e) => NextDate();

            spinnerRoute.ItemSelected += async (s, e) => { await RouteSpinner_ItemSelected(s, e); };

            await InitDateControls();
        }

        private async Task InitDateControls()
        {
            loader.StartAnimation(animation);
            try
            {
                availableDates = await tripDataService.GetAvailableDates();
                loader.ClearAnimation();
                textViewDate.Click += ShowDateDialog;
                textViewDate1.Click += ShowDateDialog;
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task LoadSettings()
        {
            settings = await userSettingsService.LoadUserSettings();
            if (settings == null)
            {
                var intent = new Intent(this, typeof(MainActivity));
                StartActivity(intent);
            }
            else
            {
                await InitSettingControls();
            }
        }

        private async Task InitSettingControls()
        {
            buttonSettings.Click += (s, e) =>
            {
                var intent = new Intent(this, typeof(MainActivity));
                intent.PutExtra("action", "edit");
                StartActivity(intent);
            };
            dateSelected = DateTime.Now;
            textViewDate.Text = dateSelected.ToShortDateString();

            try
            {
                loader.StartAnimation(animation);
                routes = (await routeDataService.GetRoutesAsync()).Select(x => new SpinnerItem(x.RouteId, x.RouteLongName)).ToList();
                loader.ClearAnimation();
                this.AddRouteSpinnerItemsWithSelectedValue(spinnerRoute, routes, settings?.RouteId, Resource.Drawable.IdSpinner);
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task RouteSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            ClearTrips();
            HideError<GenericNoConnectionException>();
            var newValue = routes.FirstOrDefault(x => x.Value == spinnerRoute.SelectedItem.ToString())?.Id;
            var from = settings.RouteId == newValue ? settings.From : selectedFromCache.ContainsKey(newValue) ? selectedFromCache[newValue] : "";
            var to = settings.RouteId == newValue ? settings.To : selectedToCache.ContainsKey(newValue) ? selectedToCache[newValue] : "";
            settings.RouteId = newValue;
            await InitStopSpinners(newValue, from, to);
        }

        private async Task ShowTrips()
        {
            ClearTrips();

            var from = settings.From;
            var to = settings.To;

            try
            {
                layoutMessageTextView.Visibility = ViewStates.Gone;
                messageTextViewYesterday.Visibility = ViewStates.Gone;
                messageTextViewNoTrips.Visibility = ViewStates.Gone;

                loader.StartAnimation(animation);
                var trips = await tripDataService.GetTripsForADay(settings.RouteId, dateSelected, from, to, settings.ShowOnlyThreeTrips);
                loader.ClearAnimation();

                if (!trips.Any())
                {
                    messageTextViewNoTrips.Visibility = ViewStates.Visible;
                    return;
                }

                var index = 0;
                foreach (var trip in trips)
                {
                    parentLayout.AddTripLayout(trip, index % 2 == 0 ? new Color(0x37, 0x42, 0x32) : new Color(0x2a, 0x5c, 0x12), Resources.DisplayMetrics.Density);
                    index++;
                }

                if (settings.ShowEligibleTrips)
                {
                    RunOnUiThread(() => CheckForEligibleTrips(from, to));
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private int DpToPx(int dp) => (int)(dp * Resources.DisplayMetrics.Density + 0.5f);

        private async void CheckForEligibleTrips(string from, string to)
        {
            try
            {
                var eligibleTrips = await tripDataService.GetEligibleTrips(settings.RouteId, dateSelected, from, to);

                var hasEligibleTrips = eligibleTrips.Any(x => x.GetDate() == dateSelected.Date);
                layoutMessageTextView.Visibility = hasEligibleTrips ? ViewStates.Visible : ViewStates.Gone;

                var hasEligibleTripsYesterday = eligibleTrips.Any(x => x.GetDate() == dateSelected.Date.AddDays(-1));
                messageTextViewYesterday.Visibility = hasEligibleTripsYesterday ? ViewStates.Visible : ViewStates.Gone;

                foreach (var eligibleTrip in eligibleTrips)
                {
                    var layout = FindViewById<LinearLayout>(Math.Abs($"{eligibleTrip.From.TripId}-layout".GetHashCode()));
                    if (layout == null) continue;
                    layout.Click += (s, e) =>
                    {
                        browserService.OpenServiceGuaranteePage(eligibleTrip, dateSelected,
                            GetStopNameById(eligibleTrip.From.StopId),
                            GetStopNameById(eligibleTrip.To.StopId),
                            settings.PrestoCardNumber);
                    };

                    layout.GetChildAt(0).Visibility = ViewStates.Gone;
                    var imageView = new ImageView(this)
                    {
                        LayoutParameters = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.MatchParent, 0.1f)
                    };
                    layout.RemoveViewAt(0);
                    imageView.SetPadding(DpToPx(15), DpToPx(3), 0, DpToPx(3));
                    imageView.SetMaxHeight(DpToPx(14));
                    imageView.SetImageResource(Resource.Drawable.sharp_monetization_on_24);
                    layout.AddView(imageView, 0);
                }
            }
            catch (Exception e)
            {
            }
        }

        #region Date methods

        private void ShowDateDialog(object s, EventArgs e)
        {
            var dialog = new DatePickerDialogFragment(this, dateSelected, this, availableDates.Item1, availableDates.Item2);
            dialog.Show(FragmentManager, "date");
        }

        public override bool DispatchTouchEvent(MotionEvent e)
        {
            switch (e.Action)
            {
                case MotionEventActions.Move:
                    var diff = e.GetX() - initialX;
                    if (Math.Abs(diff) > 50)
                    {
                        textViewDate.TranslationX = diff - Math.Sign(diff) * 50;
                        textViewDate.Alpha = 1 - Math.Abs(diff / 600);
                    }
                    if (diff > 250 && isFreshMove)
                    {
                        PreviousDate();
                        isFreshMove = false;
                    }
                    if (diff < -250 && isFreshMove)
                    {
                        NextDate();
                        isFreshMove = false;
                    }
                    break;
                case MotionEventActions.Down:
                    initialX = e.GetX();
                    isFreshMove = true;
                    break;
                case MotionEventActions.Up:
                    isFreshMove = true;
                    textViewDate.TranslationX = 0;
                    textViewDate.Alpha = 1;
                    break;
            }
            return base.DispatchTouchEvent(e);
        }

        private async void PreviousDate()
        {
            flipper.SetInAnimation(this, Resource.Animation.in_left);
            flipper.SetOutAnimation(this, Resource.Animation.out_right);
            if (currentDateTextView == textViewDate)
            {
                currentDateTextView = textViewDate1;
                dateSelected = dateSelected.AddDays(-1);
                flipper.ShowNext();
                await UpdateDate();
            }
            else
            {
                currentDateTextView = textViewDate;
                dateSelected = dateSelected.AddDays(-1);
                flipper.ShowPrevious();
                await UpdateDate();
            }
        }

        private async void NextDate()
        {
            flipper.SetInAnimation(this, Resource.Animation.in_right);
            flipper.SetOutAnimation(this, Resource.Animation.out_left);
            if (currentDateTextView == textViewDate)
            {
                currentDateTextView = textViewDate1;
                dateSelected = dateSelected.AddDays(1);
                flipper.ShowNext();
                await UpdateDate();
            }
            else
            {
                currentDateTextView = textViewDate;
                dateSelected = dateSelected.AddDays(1);
                flipper.ShowPrevious();
                await UpdateDate();
            }
        }

        public async void OnDateSet(DatePicker view, int year, int month, int dayOfMonth)
        {
            ClearTrips();
            HideError<GenericNoConnectionException>();
            dateSelected = new DateTime(year, month + 1, dayOfMonth);
            if (dateSelected <= DateTime.Now.Date)
            {
                dateSelected = new DateTime(year, month + 1, dayOfMonth, DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second);
            }
            await UpdateDate();
        }

        private async Task UpdateDate()
        {
            currentDateTextView.Text = dateSelected.ToShortDateString();
            await ShowTrips();
        }

        #endregion

        protected override void ShowError(Exception e)
        {
            TextView tv = new TextView(this);
            if (e is GenericNoConnectionException)
            {
                tv = textViewException;
            }
            else if (e is LocationIsNotAvailableException)
            {
                tv = textViewNoLocation;
            }
            tv.Text = e.Message;
            tv.Visibility = ViewStates.Visible;
        }

        protected override void HideError<T>()
        {
            if (typeof(T).IsSubclassOf(typeof(GenericNoConnectionException))
                || typeof(T) == typeof(GenericNoConnectionException))
            {
                textViewException.Visibility = ViewStates.Gone;
                textViewException.Text = "";
            }
            else if (typeof(T) == typeof(LocationIsNotAvailableException))
            {
                textViewNoLocation.Visibility = ViewStates.Gone;
                textViewNoLocation.Text = "";
            }
        }

        protected override void UpdateFromSpinnerValue(string newValue)
        {
            base.UpdateFromSpinnerValue(newValue);
            selectedFromCache[settings.RouteId] = newValue;
        }

        protected override void UpdateToSpinnerValue(string newValue)
        {
            base.UpdateToSpinnerValue(newValue);
            selectedToCache[settings.RouteId] = newValue;
        }

        private void ClearTrips()
        {
            parentLayout.RemoveAllViews();
        }
    }
}