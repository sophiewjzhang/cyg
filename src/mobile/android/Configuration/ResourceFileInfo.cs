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
using System.IO;
using System.Reflection;
using Microsoft.Extensions.FileProviders;

namespace android.Configuration
{
    public class ResourceFileInfo : IFileInfo
    {
        public ResourceFileInfo(string path)
        {
            PhysicalPath = path;
            Name = Path.GetFileName(path);
        }

        public Stream CreateReadStream()
        {
            var assembly = typeof(ResourceFileInfo).GetTypeInfo().Assembly;
            return assembly.GetManifestResourceStream(PhysicalPath);
        }

        public bool Exists
        {
            get
            {
                var assembly = typeof(ResourceFileInfo).GetTypeInfo().Assembly;
                return assembly.GetManifestResourceNames().Contains(Name);
            }
        }

        public long Length => CreateReadStream().Length;
        public string PhysicalPath { get; }
        public string Name { get; }
        public DateTimeOffset LastModified => DateTimeOffset.Now;
        public bool IsDirectory => false;
    }

}