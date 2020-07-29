namespace JoyBrick.Walkio.Game.Common
{
    using Unity.Entities;

    public interface IEcsSettingProvider
    {
        GameObjectConversionSettings RefGameObjectConversionSettings { get; }
    }
}
