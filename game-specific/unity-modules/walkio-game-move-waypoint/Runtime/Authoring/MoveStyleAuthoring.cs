// namespace JoyBrick.Walkio.Game.Move.Waypoint
// {
//     using Unity.Entities;
//     using UnityEngine;
//
//     public enum EMoveStyle
//     {
//         Controlled,
//         MoveOnWayPointPath,
//         MoveOnFlowField
//     }
//
//     public class MoveStyleAuthoring :
//         MonoBehaviour,
//         IConvertGameObjectToEntity
//     {
//         public EMoveStyle moveStyle;
//         
//         public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
//         {
//             if (moveStyle == EMoveStyle.Controlled)
//             {
//                 
//             }
//             else if (moveStyle == EMoveStyle.MoveOnWayPointPath)
//             {
//                 dstManager.AddComponentData(entity, new WaypointMoveIndication());
//             }
//             else if (moveStyle == EMoveStyle.MoveOnFlowField)
//             {
//                 dstManager.AddComponentData(entity, new FlowFieldMoveIndication());
//             }            
//         }
//     }
// }
