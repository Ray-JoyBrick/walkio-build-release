namespace JoyBrick.Walkio.Game.Hud
{
    using Unity.Entities;
    using UnityEngine;

    public class MainPanel :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
        }
    }
}