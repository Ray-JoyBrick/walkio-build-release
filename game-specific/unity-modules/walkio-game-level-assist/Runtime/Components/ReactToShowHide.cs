namespace JoyBrick.Walkio.Game.Level.Assist
{
    using Unity.Entities;

    [GenerateAuthoringComponent]
    public struct ReactToShowHide : IComponentData
    {
        public int Category;
    }
}
