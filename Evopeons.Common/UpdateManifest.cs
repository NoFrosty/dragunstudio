using System;
using System.Collections.Generic;
using System.Text;

namespace Evopeons.Common
{
    class UpdateManifest
    {
        public SemVersion Version { get; set; }
        public SemVersion FromVersion { get; set; }
        public List<FileInfo> FileInfos { get; set; }
    }
}
