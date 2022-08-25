using DragunStudio.Managers.Package.Models.Actions;
using System;

namespace DragunStudio.Managers.Package.Models
{
    public class UpdateManifest
    {
        public Guid ApplicationId { get; set; }
        public Guid ToVersionId { get; set; }
        public string ToVersion { get; set; }
        public Guid? FromVersionId { get; set; }
        public string FromVersion { get; set; }
        public FileAction[] Actions { get; set; }
    }
}