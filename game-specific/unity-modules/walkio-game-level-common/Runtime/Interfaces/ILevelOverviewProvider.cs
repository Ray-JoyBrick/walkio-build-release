namespace JoyBrick.Walkio.Game.Level
{
    using System.Collections.Generic;

    public interface ILevelOverviewProvider
    {
        List<Template.LevelOverviewDetail> LeveOverviewDetails { get; }

        void AddLevelOverviewDetail(Template.LevelOverviewDetail levelOverviewDetail);
    }
}
