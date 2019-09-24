using System;
using System.Collections.Generic;
using System.Text;

namespace Evopeons.Common
{
    public class FileInfo
    {
        public Guid Id { get; set; }
        public string FileName { get; set; }
        public string RelativePath { get; set; }
        public string Hash { get; set; }
        public long Size { get; set; }
        public PatchAction PatchAction { get; set; }
    }
}
