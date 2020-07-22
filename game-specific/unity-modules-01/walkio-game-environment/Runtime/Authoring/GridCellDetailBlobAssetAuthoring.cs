namespace JoyBrick.Walkio.Game.Environment
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    
    using GameTemplate = JoyBrick.Walkio.Game.Template;

    public class GridCellDetailBlobAssetAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        // public Bridge.TileDetailAsset tileDetailAsset;

        public List<GameTemplate.GridCellDetail> gridCellDetails;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponent<RemoveAfterConversion>(entity);
            // dstManager.AddComponentObject(entity, tileDetailAsset);
        }
    }
}
