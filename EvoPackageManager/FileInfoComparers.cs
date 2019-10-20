using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Evorine.EvoPackageManager
{
    public static class FileInfoComparers
    {
        public static HashFileInfoComparer Hash { get; } = new HashFileInfoComparer();

        public class HashFileInfoComparer : IEqualityComparer<FileInfo>
        {
            /// <summary>
            /// Returns true if file remains same but is renamed
            /// </summary>
            public bool Equals(FileInfo x, FileInfo y)
            {
                return string.Equals(x.Hash, y.Hash, StringComparison.OrdinalIgnoreCase);
            }

            public int GetHashCode(FileInfo obj)
            {
                return 0;
                return obj.GetHashCode();
            }
        }
    }
}
