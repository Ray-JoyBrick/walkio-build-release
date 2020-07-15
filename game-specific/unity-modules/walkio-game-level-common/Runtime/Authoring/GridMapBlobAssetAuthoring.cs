namespace JoyBrick.Walkio.Game.Level.Common
{
    using Unity.Entities;
    using UnityEngine;

    public class GridMapBlobAssetAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            throw new System.NotImplementedException();
        }
    }
}
