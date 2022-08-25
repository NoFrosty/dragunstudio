using DragunStudio.Managers.Package.Enums;

namespace DragunStudio.Managers.Package.Models.Actions
{
    public class CopyFileAction : FileAction
    {
        public override ActionType ActionType => ActionType.CopyFile;

        public PackageFileInfo SourceFile { get; set; }
        public PackageFileInfo DestinationFile { get; set; }
    }
}