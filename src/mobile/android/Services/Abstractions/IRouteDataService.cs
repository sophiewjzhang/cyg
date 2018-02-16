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
using DTO;
using System.Threading.Tasks;

namespace android.Services.Abstractions
{
    public interface IRouteDataService
    {
        Task<IEnumerable<Route>> GetRoutesAsync();
    }
}