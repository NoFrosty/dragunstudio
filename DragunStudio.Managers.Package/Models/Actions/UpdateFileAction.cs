using DragunStudio.Managers.Package.Enums;
using DragunStudio.Managers.Package.Models;

namespace DragunStudio.Managers.Package.Models.Actions
{
    public class UpdateFileAction : FileAction
    {
        public override ActionType ActionType => ActionType.UpdateFile;

        public PackageFileInfo OldFile { get; set; }
        public PackageFileInfo NewFile { get; set; }
        public Guid DiffId { get; set; }
        public long DiffSize { get; set; }
    }
}