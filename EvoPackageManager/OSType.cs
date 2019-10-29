using System;

namespace Evorine.EvoPackageManager
{
    [Flags]
    public enum OSType
    {
        None = 0,
        Windows = 1,
        Linux = 2,
        MacOS = 4
    }
}
