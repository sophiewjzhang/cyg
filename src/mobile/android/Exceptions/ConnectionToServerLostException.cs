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

namespace android.Exceptions
{
    public class ConnectionToServerLostException : GenericNoConnectionException
    {
        public override string Message => "Connection to server has been lost";
    }
}