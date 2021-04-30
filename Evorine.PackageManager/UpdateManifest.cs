using System;

namespace Evorine.Manager
{
    public class UpdateManifest
    {
        public Guid ApplicationId { get; set; }
        public Guid ToVersionId { get; set; }
        public string ToVersion { get; set; }
        public Guid? FromVersionId { get; set; }
        public string FromVersion { get; set; }
        public UpdateAction[] Actions { get; set; }
    }
}