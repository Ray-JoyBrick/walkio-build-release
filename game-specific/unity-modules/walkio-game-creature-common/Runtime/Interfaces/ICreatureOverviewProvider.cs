namespace JoyBrick.Walkio.Game.Creature
{
    using System.Collections.Generic;


    public interface ICreatureOverviewProvider
    {
        List<Template.TeamLeaderOverview> TeamLeaderOverviews { get; }
        List<Template.TeamMinionOverview> TeamMinionOverviews { get; }
        
        void AddTeamLeaderOverview(Template.TeamLeaderOverview teamLeaderOverview);
        void AddTeamMinionOverview(Template.TeamMinionOverview teamMinionOverview);
    }
}
