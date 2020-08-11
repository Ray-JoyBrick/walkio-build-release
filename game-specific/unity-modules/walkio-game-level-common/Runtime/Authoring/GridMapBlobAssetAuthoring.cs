namespace JoyBrick.Walkio.Game.Level
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;
    
    //
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;

    public class GridMapBlobAssetAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public Vector2Int gridCellCount;
        public Vector2 gridCellSize;

        public List<int> gridCells;

        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new GameFlowControl.StageUse());
        }
    }
}
