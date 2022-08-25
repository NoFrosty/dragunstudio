using DragunStudio.Managers.Package.Models;
using System;
using System.Collections.Generic;

namespace DragunStudio.Managers.Package.Helpers
{
    public static class FileInfoComparer
    {
        public static HashFileInfoComparer Hash { get; } = new HashFileInfoComparer();

        public class HashFileInfoComparer : IEqualityComparer<PackageFileInfo>
        {
            /// <summary>
            /// Returns true if file remains same but is renamed
            /// </summary>
            public bool Equals(PackageFileInfo x, PackageFileInfo y)
            {
                return string.Equals(x.Hash, y.Hash, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(PackageFileInfo obj) => obj.GetHashCode();
        }
    }
}