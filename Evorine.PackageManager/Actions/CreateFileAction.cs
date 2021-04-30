using Evorine.Manager.DataObjects;

namespace Evorine.Manager.Actions
{
    public class CreateFileAction : UpdateAction
    {
        public override ActionType ActionType => ActionType.CreateFile;

        public PackageFileInfo NewFile { get; set; }
    }
}