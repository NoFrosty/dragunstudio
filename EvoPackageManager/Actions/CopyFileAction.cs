using System;
using System.Collections.Generic;
using System.Text;

namespace Evorine.EvoPackageManager.Actions
{
    public class CopyFileAction : UpdateAction
    {
        public override ActionType ActionType => ActionType.CopyFile;

        public FileInfo SourceFile { get; set; }
        public FileInfo DestinationFile { get; set; }
    }
}
