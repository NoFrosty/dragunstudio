using Evorine.EvoPackageManager.DataObjects;

namespace Evorine.EvoPackageManager.Actions
{
    public class CreateFileAction : UpdateAction
    {
        public override ActionType ActionType => ActionType.CreateFile;

        public PackageFileInfo NewFile { get; set; }
    }
}