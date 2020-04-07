using Evorine.EvoPackageManager.DataObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Evorine.EvoPackageManager.Actions
{
    public class CopyFileAction : UpdateAction
    {
        public override ActionType ActionType => ActionType.CopyFile;

        public PackageFileInfo SourceFile { get; set; }
        public PackageFileInfo DestinationFile { get; set; }
    }
}
