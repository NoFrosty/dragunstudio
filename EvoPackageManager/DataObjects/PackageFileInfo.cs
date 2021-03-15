using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.IO;

namespace Evorine.EvoPackageManager.DataObjects
{
    [Table("File")]
    public class PackageFileInfo : IEquatable<PackageFileInfo>
    {
        [ExplicitKey]
        public Guid FileId { get; set; }

        public Guid VersionId { get; set; }
        public string FileName { get; set; }
        public string DirectoryPath { get; set; }
        public string Hash { get; set; }
        public long Size { get; set; }
        public long CompressedSize { get; set; }

        [Computed]
        public string RelativeFilename => Path.Combine(DirectoryPath, FileName);

        public override bool Equals(object obj)
        {
            return obj is PackageFileInfo info && Equals(info);
        }

        public bool Equals(PackageFileInfo other)
        {
            return FileName == other.FileName &&
                   DirectoryPath == other.DirectoryPath &&
                   Hash == other.Hash;
        }

        public override int GetHashCode()
        {
            var hashCode = -798947376;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(FileName);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(DirectoryPath);
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Hash);
            return hashCode;
        }

        public override string ToString() => RelativeFilename;
    }
}