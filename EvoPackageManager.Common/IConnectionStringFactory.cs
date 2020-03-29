using System;
using System.Collections.Generic;
using System.Text;

namespace Evorine.EvoPackageManager
{
    public interface IConnectionStringFactory
    {
        string GetConnectionString();
    }
}
