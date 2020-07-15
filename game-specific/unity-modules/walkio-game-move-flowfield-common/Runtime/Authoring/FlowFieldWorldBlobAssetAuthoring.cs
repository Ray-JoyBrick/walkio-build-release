namespace JoyBrick.Walkio.Game.Move.FlowField.Common
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;

    public class FlowFieldWorldBlobAssetAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        [System.Serializable]
        public class Context
        {
            public Vector2Int gridTileCount;
            public Vector2Int gridTileCellCount;

            public List<int> gridCellDetals;
        }

        public Context context;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
        }
    }
}
