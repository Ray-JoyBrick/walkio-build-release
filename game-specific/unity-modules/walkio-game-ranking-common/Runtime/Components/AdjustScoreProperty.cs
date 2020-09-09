namespace JoyBrick.Walkio.Game.Ranking
{
    using Unity.Entities;

    public struct AdjustScoreProperty : IComponentData
    {
        public int Id;
        public int Score;
    }
}