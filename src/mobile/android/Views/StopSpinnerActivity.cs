using android.extensions;
using Android.App;
using Android.Graphics;
using Android.Views.Animations;
using Android.Widget;
using Autofac;
using DTO;
using models;
using services.abstractions;
using services.abstractions.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace android.Views
{
    public abstract class StopSpinnerActivity : Activity
    {
        protected abstract void ShowError(Exception e);
        protected abstract void HideError<T>();
        private IStopDataService stopDataService;
        private IList<SpinnerItem> stops;
        private IEnumerable<Stop> stopModels;
        private Spinner spinnerFrom;
        private Spinner spinnerTo;
        private ImageButton buttonSwitch;
        private Func<Task> spinnerSelect;
        protected UserSettings settings;
        private bool useIds = true;
        private bool swapLocationNeeded = false;
        protected TextView loader;
        protected TranslateAnimation animation;

        protected void InitControls(Func<Task> spinnerSelect, bool swapLocationNeeded, bool useIds = true)
        {
            stopDataService = App.Container.Resolve<IStopDataService>();
            this.spinnerSelect = spinnerSelect;
            this.useIds = useIds;
            this.swapLocationNeeded = swapLocationNeeded;

            spinnerFrom = FindViewById<Spinner>(Resource.Id.spinnerFrom);
            spinnerFrom.ItemSelected += async (s, e) => { await FromSpinner_ItemSelected(s, e); };

            spinnerTo = FindViewById<Spinner>(Resource.Id.spinnerTo);
            spinnerTo.ItemSelected += async (s1, e1) => { await ToSpinner_ItemSelected(s1, e1); };

            buttonSwitch = FindViewById<ImageButton>(Resource.Id.buttonSwitch);
            buttonSwitch.Click += async (s1, e1) => { await ButtonSwitch_Click(s1, e1); };

            loader = FindViewById<TextView>(Resource.Id.loader);

            var size = new Point();
            WindowManager.DefaultDisplay.GetSize(size);

            animation = new TranslateAnimation(0, size.X - 100, 0, 0);
            animation.Duration = 1000;
            animation.FillAfter = true;
            animation.RepeatMode = RepeatMode.Restart;
            animation.RepeatCount = int.MaxValue;
        }

        protected async Task InitStopSpinners(string routeId, string from, string to)
        {
            try
            {
                loader.StartAnimation(animation);
                stopModels = await stopDataService.GetStopsByRoute(routeId);
                loader.ClearAnimation();
                stops = stopModels.Select(x => new SpinnerItem(x.StopId, x.StopName)).ToList();

                from = string.IsNullOrEmpty(from) ? stops.FirstOrDefault()?.Id : from;
                // select union station by default, otherwise second station in the list
                to = string.IsNullOrEmpty(to) ? stops.FirstOrDefault(x => x.Id == "UN")?.Id ?? stops[1]?.Id : to;

                if (swapLocationNeeded && settings.SwapDirectionBasedOnLocation)
                {
                    try
                    {
                        HideError<LocationIsNotAvailableException>();
                        if (await IsSwitchRequiredBasedOnLocation(from, to))
                        {
                            var temp = to;
                            to = from;
                            from = temp;
                        }
                    }
                    catch (LocationIsNotAvailableException e)
                    {
                        ShowError(e);
                    }
                }

                UpdateFromSpinnerValue(from);
                UpdateToSpinnerValue(to);

                SyncSpinner(spinnerTo, settings.To, settings.From);
                SyncSpinner(spinnerFrom, settings.From, settings.To);

                await spinnerSelect();
            }
            catch (Exception ex)
            {
                ShowError(ex);
            }
        }

        private async Task<bool> IsSwitchRequiredBasedOnLocation(string from, string to, Location location = null)
        {
            loader.StartAnimation(animation);
            var closerStop = await stopDataService.GetCloserStop(stopModels.Where(x => x.StopId == from || x.StopId == to), OnLocationChange, location);
            loader.ClearAnimation();
            return closerStop != null && closerStop.StopId != from;
        }

        protected async void OnLocationChange(models.Location location)
        {
            HideError<LocationIsNotAvailableException>();
            if (await IsSwitchRequiredBasedOnLocation(settings.From, settings.To, location))
            {
                await ButtonSwitch_Click(null, null);
            }
        }

        protected virtual void UpdateFromSpinnerValue(string newValue)
        {
            settings.From = newValue;
        }

        protected virtual void UpdateToSpinnerValue(string newValue)
        {
            settings.To = newValue;
        }

        private async Task ButtonSwitch_Click(object sender, EventArgs e)
        {
            HideError<GenericNoConnectionException>();
            var temp = settings.From;
            UpdateFromSpinnerValue(settings.To);
            UpdateToSpinnerValue(temp);

            SyncSpinner(spinnerFrom, settings.From, settings.To);
            SyncSpinner(spinnerTo, settings.To, settings.From);

            await spinnerSelect();
        }

        protected async Task FromSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            HideError<GenericNoConnectionException>();
            var newValue = stops.FirstOrDefault(x => x.Value == spinnerFrom.SelectedItem.ToString()).Id;
            if (newValue != settings.From)
            {
                UpdateFromSpinnerValue(newValue);
                SyncSpinner(spinnerTo, settings.To, settings.From);
                await spinnerSelect();
            }
        }

        protected async Task ToSpinner_ItemSelected(object sender, AdapterView.ItemSelectedEventArgs e)
        {
            HideError<GenericNoConnectionException>();
            var newValue = stops.FirstOrDefault(x => x.Value == spinnerTo.SelectedItem.ToString()).Id;
            if (newValue != settings.To)
            {
                UpdateToSpinnerValue(newValue);
                SyncSpinner(spinnerFrom, settings.From, settings.To);
                await spinnerSelect();
            }
        }

        private void SyncSpinner(Spinner spinner, string currentValue, string valueToExclude)
        {
            var newItems = new List<SpinnerItem>(stops);
            newItems.RemoveAll(x => x.Id == valueToExclude);
            if (this.useIds)
            {
                this.AddStopSpinnerItemsWithSelectedValue(spinner, newItems, currentValue, Resource.Id.spinnerFrom, Resource.Drawable.stop_spinner_bg_right, Resource.Drawable.stop_spinner_bg_left, Resource.Drawable.IdSpinner);
            }
            else
            {
                this.AddRouteSpinnerItemsWithSelectedValue(spinner, newItems, currentValue, Resource.Drawable.IdSpinner);
            }
        }
    }
}