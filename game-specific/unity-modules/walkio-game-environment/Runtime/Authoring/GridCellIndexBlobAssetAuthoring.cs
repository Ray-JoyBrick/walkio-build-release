namespace JoyBrick.Walkio.Game.Environment
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    using GameTemplate = JoyBrick.Walkio.Game.Template;

    public class GridCellIndexBlobAssetAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        // public GameTemplate.GridCellData gridCellData;
        
        public List<int> indices;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<RemoveAfterConversion>(entity);
            // dstManager.AddComponentObject(entity, gridCellData);
        }
    }
}
