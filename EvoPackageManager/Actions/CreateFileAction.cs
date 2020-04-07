using Evorine.EvoPackageManager.DataObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Evorine.EvoPackageManager.Actions
{
    public class CreateFileAction : UpdateAction
    {
        public override ActionType ActionType => ActionType.CreateFile;

        public PackageFileInfo NewFile { get; set; }
    }
}
