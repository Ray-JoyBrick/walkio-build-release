namespace JoyBrick.Walkio.Game.Common
{
    using Unity.Entities;

    public struct LevelSetting : IComponentData
    {
        public int HorizontalCellCount;
        public int VerticalCellCount;
    }
}
