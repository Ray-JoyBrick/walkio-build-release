namespace JoyBrick.Walkio.Game.Environment.Creature
{
    using Unity.Entities;
    using UnityEngine;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public enum EUnitMovementStyle
    {
        MoveOnWaypointPath,
        MoveOnAStarPath,
        MoveOnFlowFieldTile,
        MoveOnCollider
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

            if (unitMovementStyle == EUnitMovementStyle.MoveOnWaypointPath)
            {
                dstManager.AddComponentData(entity, new MoveOnWaypointPath
                {
                    StartIndex = startPathIndex,
                    EndIndex = endPathIndex,

                    AtIndex = startPathIndex
                });
            }
            else if (unitMovementStyle == EUnitMovementStyle.MoveOnAStarPath)
            {
                
            }
            else if (unitMovementStyle == EUnitMovementStyle.MoveOnFlowFieldTile)
            {
                
            }
            else if (unitMovementStyle == EUnitMovementStyle.MoveOnCollider)
            {
                
            }

             // For cleanup use
             dstManager.AddComponentData(entity, new GameCommon.StageUse());
            
//             var neutralForceAuthoring = GetComponent<NeutralForceAuthoring>();
//             var teamForceAuthoring = GetComponent<TeamForceAuthoring>();
//             var teamLeaderAuthoring = GetComponent<TeamLeaderAuthoring>();

//             if (teamLeaderAuthoring != null)
//             {
//                 // Let Team Leader Authoring do the naming
//             }
//             else if (teamForceAuthoring != null)
//             {
//                 dstManager.AddComponentData(entity, new GameEnvironment.MoveOnFlowFieldTile());
//                 dstManager.AddComponentData(entity, new GameEnvironment.MoveOnFlowFieldTileInfo());
                
// #if UNITY_EDITOR
//                 dstManager.SetName(entity, "Team Unit");
// #endif
//             }
//             else if (neutralForceAuthoring != null)
//             {
// #if UNITY_EDITOR
//                 dstManager.SetName(entity, "Neutral Unit");
// #endif
//             }
        }
    }
}
