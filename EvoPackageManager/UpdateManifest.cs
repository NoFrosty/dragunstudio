using System.Collections.Generic;

namespace Evorine.EvoPackageManager
{
    class UpdateManifest
    {
        public string Version { get; set; }
        public string FromVersion { get; set; }
        public List<FileInfo> FileInfos { get; set; }
    }
}
