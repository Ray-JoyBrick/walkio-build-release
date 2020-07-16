namespace JoyBrick.Walkio.Game.Level
{
    using Unity.Entities;
    using UnityEngine;

    public class GridMapBlobAssetAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
        }
    }
}
