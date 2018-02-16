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
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace android.Configuration
{
    public class ResourceFileProvider : IFileProvider
    {
        public IFileInfo GetFileInfo(string subpath)
        {
            return new ResourceFileInfo(subpath);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            return new ResourceDirectoryContents();
        }

        public IChangeToken Watch(string filter)
        {
            return new ResourceChangeToken();
        }

    }

}