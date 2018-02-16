using System;
using System.Collections.Generic;
using Microsoft.Extensions.FileProviders;
using System.Collections;

namespace android.Configuration
{
    public class ResourceDirectoryContents : IDirectoryContents
    {
        public IEnumerator<IFileInfo> GetEnumerator()
        {
            throw new NotImplementedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public bool Exists { get; }
    }

}