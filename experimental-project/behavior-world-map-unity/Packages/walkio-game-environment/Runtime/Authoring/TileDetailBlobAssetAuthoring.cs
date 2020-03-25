namespace JoyBrick.Walkio.Game.Environment
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    public class TileDetailBlobAssetAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public Bridge.TileDetailAsset tileDetailAsset;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<RemoveAfterConversion>(entity);
            dstManager.AddComponentObject(entity, tileDetailAsset);
        }
    }
}