using DragunStudio.Managers.Package.Enums;

namespace DragunStudio.Managers.Package.Models.Actions
{
    public class DeleteFileAction : FileAction
    {
        public override ActionType ActionType => ActionType.DeleteFile;

        public PackageFileInfo File { get; set; }
    }
}