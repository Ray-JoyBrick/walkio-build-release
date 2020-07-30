namespace JoyBrick.Walkio.Game.Hud.Stage
{
    using GameCommand = JoyBrick.Walkio.Game.Command;
    
    public class TeamUnitCountInfo : GameCommand.IInfo
    {
        public int TeamId { get; set; }
        public int Count { get; set; }

        public override string ToString()
        {
            var desc = $"TeamId: {TeamId} Count: {Count}";
            return desc;
        }
    }
}
