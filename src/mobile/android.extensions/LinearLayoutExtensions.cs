using Android.Graphics;
using Android.Views;
using Android.Widget;
using DTO;
using System;
using Android.Content.Res;
using Orientation = Android.Widget.Orientation;

namespace android.extensions
{
    public static class LinearLayoutExtensions
    {
        public static void AddTripLayout(this LinearLayout parentLayout, TripFromTo trip, Color backgroundColor, float density)
        {
            var linearLayout = new LinearLayout(parentLayout.Context)
            {
                LayoutParameters = new LinearLayout.LayoutParams(ViewGroup.LayoutParams.FillParent,
                    ViewGroup.LayoutParams.WrapContent),
                Orientation = Orientation.Horizontal,
                Id = Math.Abs($"{trip.From.TripId}-layout".GetHashCode())
            };
            parentLayout.AddView(linearLayout);

            linearLayout.AddView(parentLayout.Context.GetTextViewTripListStyle(trip.From.DepartureTime.ToString("hh':'mm"),
                backgroundColor, density, id: $"{trip.From.TripId}-from"));

            linearLayout.AddView(parentLayout.Context.GetTextViewTripListStyle(trip.To.ArrivalTime.ToString("hh':'mm"),
                backgroundColor, density, id: $"{trip.From.TripId}-to"));

            linearLayout.AddView(parentLayout.Context.GetTextViewTripListStyle(trip.GetTripTimeText(), backgroundColor,
                density, id: $"{trip.From.TripId}-duration"));

        }
    }
}