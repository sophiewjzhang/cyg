using android.Exceptions;
using android.extensions;
using android.Services.Abstractions;
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
using static Android.App.DatePickerDialog;
using static Android.Views.GestureDetector;
using static Android.Views.View;

namespace android
{
    [Activity(Label = "Trips")]
    public class TripsActivity : StopSpinnerActivity, IOnDateSetListener
    {
        private IRouteDataService routeDataService;
        private ITripDataService tripDataService;
        private ImageButton buttonSettings;
        private Spinner spinnerRoute;
        private TextView textViewDate;
        private TextView textViewDate1;
        private TextView textViewException;
        private TextView textViewNoLocation;
        private IUserSettingsService userSettingsService;
        private DateTime dateSelected;
        private IList<SpinnerItem> routes;
        private LinearLayout parentLayout;
        private ViewFlipper flipper;
        private float initialX;
        private TextView currentDateTextView;
        private bool isFreshMove = false;
        private ImageButton buttonLeft;
        private ImageButton buttonRight;

        private Dictionary<string, string> selectedFromCache = new Dictionary<string, string>();
        private Dictionary<string, string> selectedToCache = new Dictionary<string, string>();

        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);

            SetContentView(Resource.Layout.Trips);

            ResolveDependencies();
            InitControls();
            await LoadSettings();
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
            buttonLeft = FindViewById<ImageButton>(Resource.Id.buttonLeft);
            buttonRight = FindViewById<ImageButton>(Resource.Id.buttonRight);

            buttonLeft.Click += ButtonLeft_Click;
            buttonRight.Click += ButtonRight_Click;

            spinnerRoute.ItemSelected += async (s, e) => { await RouteSpinner_ItemSelected(s, e); };
            loader.StartAnimation(animation);
            try
            {
                var availableDates = await tripDataService.GetAvailableDates();
                loader.ClearAnimation();

                textViewDate.Click += (s, e) =>
                {

                    var dialog = new DatePickerDialogFragment(this, dateSelected, this, availableDates.Item1, availableDates.Item2);
                    dialog.Show(FragmentManager, "date");
                };
                textViewDate1.Click += (s, e) =>
                {
                    var dialog = new DatePickerDialogFragment(this, dateSelected, this, availableDates.Item1, availableDates.Item2);
                    dialog.Show(FragmentManager, "date");
                };
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private void ButtonLeft_Click(object sender, EventArgs e)
        {
            PreviousDate();
        }

        private void ButtonRight_Click(object sender, EventArgs e)
        {
            NextDate();
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

        private void ResolveDependencies()
        {
            userSettingsService = App.Container.Resolve<IUserSettingsService>();
            routeDataService = App.Container.Resolve<IRouteDataService>();
            tripDataService = App.Container.Resolve<ITripDataService>();
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
            var newValue = routes.FirstOrDefault(x => x.Value == spinnerRoute.SelectedItem.ToString()).Id;
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
                IEnumerable<TripFromTo> trips;
                loader.StartAnimation(animation);
                if (settings.ShowOnlyThreeTrips)
                {
                    trips = await tripDataService.GetNextThreeTrips(settings.RouteId, dateSelected, from, to);
                }
                else
                {
                    trips = await tripDataService.GetTripsFromTo(settings.RouteId, dateSelected, from, to);
                }
                loader.ClearAnimation();

                parentLayout.RemoveAllViews();

                if (!trips.Any())
                {
                    var textView = new TextView(this)
                    {
                        LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.MatchParent, LinearLayout.LayoutParams.WrapContent),
                        Text = "No trips for today. Swipe left to see trips for tomorrow."
                    };
                    textView.SetTextColor(Color.White);
                    textView.SetTextSize(Android.Util.ComplexUnitType.Dip, 17);
                    textView.SetBackgroundColor(new Color(0x37, 0x42, 0x32));
                    textView.SetPadding(DpToPx(20), DpToPx(5), DpToPx(5), DpToPx(5));
                    parentLayout.AddView(textView);
                }

                var index = 0;
                foreach (var trip in trips)
                {
                    Color backgroundColor = index % 2 == 0 ? new Color(0x37, 0x42, 0x32) : new Color(0x2a, 0x5c, 0x12);
                    var linearLayout = new LinearLayout(this)
                    {
                        LayoutParameters = new LinearLayout.LayoutParams(LinearLayout.LayoutParams.FillParent, LinearLayout.LayoutParams.WrapContent),
                        Orientation = Orientation.Horizontal
                    };
                    parentLayout.AddView(linearLayout);

                    var textView = new TextView(this)
                    {
                        LayoutParameters = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.MatchParent, 0.3f),
                        Text = trip.From.DepartureTime.ToString("hh':'mm")
                    };
                    textView.SetTextColor(Color.White);
                    textView.SetTextSize(Android.Util.ComplexUnitType.Dip, 17);
                    textView.SetBackgroundColor(backgroundColor);
                    textView.SetPadding(DpToPx(20), DpToPx(5), DpToPx(5), DpToPx(5));
                    linearLayout.AddView(textView);

                    var textView1 = new TextView(this)
                    {
                        LayoutParameters = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.MatchParent, 0.3f),
                        Text = trip.To.ArrivalTime.ToString("hh':'mm")
                    };
                    textView1.SetBackgroundColor(backgroundColor);
                    textView1.SetTextColor(Color.White);
                    textView1.SetPadding(DpToPx(20), DpToPx(5), DpToPx(5), DpToPx(5));
                    textView1.SetTextSize(Android.Util.ComplexUnitType.Dip, 17);
                    linearLayout.AddView(textView1);

                    var textView2 = new TextView(this)
                    {
                        LayoutParameters = new LinearLayout.LayoutParams(0, LinearLayout.LayoutParams.MatchParent, 0.3f),
                        Text = GetTripTimeText(trip)
                    };
                    textView2.SetTextColor(Color.White);
                    textView2.SetBackgroundColor(backgroundColor);
                    textView2.SetPadding(DpToPx(20), DpToPx(5), DpToPx(5), DpToPx(5));
                    textView2.SetTextSize(Android.Util.ComplexUnitType.Dip, 17);
                    linearLayout.AddView(textView2);

                    index++;
                }
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        public int DpToPx(int dp)
        {
            return (int)(dp * Resources.DisplayMetrics.Density + 0.5f);
        }

        private string GetTripTimeText(TripFromTo trip)
        {
            var tripTimeSpan = GetTripTime(trip.From.DepartureTime, trip.To.ArrivalTime);
            var hours = Math.Abs(tripTimeSpan.Hours);
            var minutes = Math.Abs(tripTimeSpan.Minutes);
            if (hours > 0)
            {
                return $"{hours} h {minutes} min";
            }
            return $"{minutes} min";
        }

        private TimeSpan GetTripTime(TimeSpan departure, TimeSpan arrival)
        {
            // + 24 to cover case when departure 23:** and arrival 00:**
            if (departure > arrival)
            {
                arrival = new TimeSpan(arrival.Hours + 24, arrival.Minutes, arrival.Seconds);
            }
            return arrival - departure;
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