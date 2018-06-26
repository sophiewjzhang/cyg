using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace android.extensions
{
    public class SpinnerItem : IEquatable<SpinnerItem>
    {
        private readonly string id;
        private readonly string value;

        public SpinnerItem(string id, string value)
        {
            this.id = id;
            this.value = value;
        }

        public string Id
        {
            get
            {
                return id;
            }
        }

        public string Value
        {
            get
            {
                return value;
            }
        }

        public bool Equals(SpinnerItem other)
        {
            return other != null && other.Id == this.Id;
        }

        public override string ToString() => value;
    }
}