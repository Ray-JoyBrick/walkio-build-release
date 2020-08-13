namespace JoyBrick.Walkio.Game.Level
{
    using System.Collections.Generic;

    public class TeamLeaderLevelInfo
    {
        public int Id { get; set; }
        public int Score { get; set; }
    }
    
    public interface ITeamLeaderLevelInfoProvider
    {
        List<TeamLeaderLevelInfo> TeamLeaderLevelInfos { get; }

        void SetupTeamLeaderLevelInfo();
    }
}
