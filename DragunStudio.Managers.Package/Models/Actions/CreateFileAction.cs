using DragunStudio.Managers.Package.Enums;

namespace DragunStudio.Managers.Package.Models.Actions
{
    public class CreateFileAction : FileAction
    {
        public override ActionType ActionType => ActionType.CreateFile;

        public PackageFileInfo NewFile { get; set; }
    }
}