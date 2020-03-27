namespace JoyBrick.Walkio.Game.Environment
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    public class TileDetailIndexBlobAssetAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        // [System.Serializable]
        // public struct WrapIntList
        // {
        //     public List<int> indices;
        // }
        
        public List<int> indices;
        // public WrapIntList wrapIntList;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            // dstManager.AddComponentObject(entity, wrapIntList);
            dstManager.AddComponent<RemoveAfterConversion>(entity);
        }
    }
}