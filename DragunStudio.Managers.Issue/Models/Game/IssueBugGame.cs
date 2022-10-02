using DragunStudio.Managers.Common;

namespace DragunStudio.Managers.Issue.Models.Game
{
    public class IssueBugGame : IssueBug
    {
        public float PlayerPositionX { get; set; }
        public float PlayerPositionY { get; set; }
        public float PlayerPositionZ { get; set; }
        public string Log { get; set; }
        public OSType OSPlatform { get; set; }
    }
}
