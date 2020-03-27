namespace JoyBrick.Walkio.Game.Navigation
{
    using Unity.Entities;

    // About the entity used in PathfindTileBuffer
    
    public struct PathfindCellBuffer : IBufferElementData
    {
        public int Value;
    }
    
    //
    public struct PathfindTileBuffer : IBufferElementData
    {
        public Entity Value;
    }
}