using Dapper.Contrib.Extensions;
using DragunStudio.Managers.Package.Enums;

namespace DragunStudio.Managers.Package.Models.Actions
{
    public abstract class FileAction
    {
        public abstract ActionType ActionType { get; }
    }
}