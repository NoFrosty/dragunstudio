using Evorine.EvoPackageManager.DataObjects;

namespace Evorine.EvoPackageManager.Actions
{
    public class DeleteFileAction : UpdateAction
    {
        public override ActionType ActionType => ActionType.DeleteFile;

        public PackageFileInfo File { get; set; }
    }
}