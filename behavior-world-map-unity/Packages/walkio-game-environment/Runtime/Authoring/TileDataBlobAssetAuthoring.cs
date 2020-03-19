namespace JoyBrick.Walkio.Game.Environment
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [System.Serializable]
    public class TileDataEx
    {
        public int kind;
        public int cost;
    }
    
    [System.Serializable]
    public class TileDataAssetEx : IComponentData
    {
        public List<TileDataEx> tileDatas;
    }

    public class TileDataBlobAssetAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public Bridge.TileDataAsset tileDataAsset;
        // public TileDataAssetEx tileDataAsset;

        // public AssetReference tileDataAssetRef;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // dstManager.AddComponentData(entity, tileDataAsset);
            dstManager.AddComponentObject(entity, tileDataAsset);
            // dstManager.AddComponentObject(entity, tileDataAssetRef.Asset);
        }
    }
}