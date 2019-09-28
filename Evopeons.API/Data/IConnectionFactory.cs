using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace Evopeons.API.Data
{
    public interface IConnectionFactory
    {
        IDbConnection GetConnection();
    }
}
