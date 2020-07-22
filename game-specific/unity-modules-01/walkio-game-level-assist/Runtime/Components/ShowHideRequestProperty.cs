namespace JoyBrick.Walkio.Game.Level.Assist
{
    using Unity.Entities;

    public struct ShowHideRequestProperty : IComponentData
    {
        public int Category;
        public bool Hide;
    }
}
