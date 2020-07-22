namespace JoyBrick.Walkio.Game.Level
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;

    public class GridMapBlobAssetAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public Vector2Int gridCellCount;
        public Vector2 gridCellSize;

        public List<int> gridCells;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
        }
    }
}
