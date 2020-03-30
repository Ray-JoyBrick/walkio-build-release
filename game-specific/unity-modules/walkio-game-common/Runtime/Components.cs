namespace JoyBrick.Walkio.Game.Common
{
    using Unity.Entities;

    public struct LoadWorldMapRequest : IComponentData
    {
        public int WorldMapIndex;
    }

    public struct LoadZoneRequest : IComponentData
    {
        public int ZoneIndex;
    }
}
