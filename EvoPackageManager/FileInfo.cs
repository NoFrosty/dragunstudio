using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;
using System.Text;

namespace Evorine.EvoPackageManager
{
    [Table("File")]
    public class FileInfo
    {
        [ExplicitKey]
        public string FileId { get; set; }
        public string VersionId { get; set; }
        public string FileName { get; set; }
        public string DirectoryPath { get; set; }
        public string Hash { get; set; }
        public long Size { get; set; }
        public long CompressedSize { get; set; }
    }
}
