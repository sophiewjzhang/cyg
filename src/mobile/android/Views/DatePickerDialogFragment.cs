using System;

using Android.App;
using Android.Content;
using Android.OS;

namespace android
{
    public class DatePickerDialogFragment : DialogFragment
    {
        private readonly Context _context;

        private DateTime _minDate;
        private DateTime _maxDate;
        private DateTime _date;

        private readonly DatePickerDialog.IOnDateSetListener _listener;


        public DatePickerDialogFragment(Context context, DateTime date, DatePickerDialog.IOnDateSetListener listener, DateTime minDate, DateTime maxDate)
        {
            _context = context;
            _date = date;
            _listener = listener;
            _minDate = minDate;
            _maxDate = maxDate;
        }

        private long ConvertToUnixTimestamp(DateTime dateTime)
        {
            var origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var diff = dateTime.ToUniversalTime() - origin;
            return (long)Math.Floor(diff.TotalMilliseconds);
        }

        public override Dialog OnCreateDialog(Bundle savedState)
        {
            var dialog = new DatePickerDialog(_context, 16973939, _listener, _date.Year, _date.Month - 1, _date.Day);
#pragma warning disable CS0618
            dialog.DatePicker.SpinnersShown = true;
            dialog.DatePicker.CalendarViewShown = false;
#pragma warning restore CS0618
            dialog.DatePicker.MaxDate = ConvertToUnixTimestamp(_maxDate);
            dialog.DatePicker.MinDate = ConvertToUnixTimestamp(_minDate);
            return dialog;
        }
    }
}