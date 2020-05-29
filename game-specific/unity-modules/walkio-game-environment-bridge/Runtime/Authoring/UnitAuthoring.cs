namespace JoyBrick.Walkio.Game.Environment.Creature
{
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public enum EUnitMovementStyle
    {
        ControlledMove,
        MoveOnWaypointPath,
        // MoveOnAStarPath,
        MoveOnFlowFieldTile
        // MoveOnCollider
    }
    
    public class UnitAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public EUnitMovementStyle unitMovementStyle;

        //
        public int startPathIndex;
        public int endPathIndex;
        public Vector3 startingPosition;
        
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new Unit());

            if (unitMovementStyle == EUnitMovementStyle.ControlledMove)
            {
            }
            else if (unitMovementStyle == EUnitMovementStyle.MoveOnWaypointPath)
            {
                // startingPosition = transform.position;
                
                transform.position = new Vector3(startingPosition.x, startingPosition.y, startingPosition.z);

                dstManager.AddComponentData(entity, new MoveOnWaypointPath
                {
                    StartIndex = startPathIndex,
                    EndIndex = endPathIndex,

                    AtIndex = startPathIndex
                });
                
                // dstManager.SetComponentData(entity, new Translation
                // {
                //     Value = (float3)startingPosition
                // });
            }
            else if (unitMovementStyle == EUnitMovementStyle.MoveOnFlowFieldTile)
            {
                dstManager.AddComponentData(entity, new MoveOnFlowFieldTile());
                dstManager.AddComponentData(entity, new MoveOnFlowFieldTileInfo());
            }

            // For cleanup use
             dstManager.AddComponentData(entity, new GameCommon.StageUse());

#if UNITY_EDITOR
            var entityName = dstManager.GetName(entity);
            dstManager.SetName(entity, $"{entityName} Unit");
#endif

        }
    }
}
