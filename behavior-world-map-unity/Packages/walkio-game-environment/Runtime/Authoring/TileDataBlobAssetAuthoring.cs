namespace JoyBrick.Walkio.Game.Environment
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;

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
        // public Bridge.TileDataAsset tileDataAsset;
        public TileDataAssetEx tileDataAsset;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, tileDataAsset);
        }
    }
}