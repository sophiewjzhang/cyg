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
using Microsoft.Extensions.Primitives;
using Autofac.Util;

namespace android.Configuration
{
    public class ResourceChangeToken : IChangeToken
    {
        public IDisposable RegisterChangeCallback(Action<object> callback, object state) => new Disposable();
        public bool HasChanged => false;
        public bool ActiveChangeCallbacks => false;
    }
}