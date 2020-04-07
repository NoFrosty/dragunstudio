using Evorine.EvoPackageManager.DataObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Evorine.EvoPackageManager.Actions
{
    public class DeleteFileAction : UpdateAction
    {
        public override ActionType ActionType => ActionType.DeleteFile;

        public PackageFileInfo File { get; set; }
    }
}
