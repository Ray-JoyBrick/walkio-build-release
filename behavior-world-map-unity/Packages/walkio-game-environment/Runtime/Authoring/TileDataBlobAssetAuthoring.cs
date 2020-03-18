namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Entities;
    using UnityEngine;

    public class TileDataBlobAssetAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public Bridge.TileDataAsset tileDataAsset;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, tileDataAsset);
        }
    }
}