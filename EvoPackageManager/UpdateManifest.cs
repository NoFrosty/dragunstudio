using System.Collections.Generic;

namespace Evorine.EvoPackageManager
{
    public class UpdateManifest
    {
        public string ApplicationId { get; set; }
        public string ToVersionId { get; set; }
        public string ToVersion { get; set; }
        public string FromVersionId { get; set; }
        public string FromVersion { get; set; }
        public UpdateAction[] Actions { get; set; }
    }
}
