namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;

    //
#if WALKIO_MOVE_FLOWFIELD
    using GameMoveFlowField = JoyBrick.Walkio.Game.Move.FlowField;
#endif
#if WALKIO_MOVE_FLOWFIELD_ASSIST
    using GameMoveFlowFieldAssist = JoyBrick.Walkio.Game.Move.FlowField.Assist;
#endif

    public partial class Bootstrap
    {
        private void _ShowPoints(int groupId, List<Vector3> points, float timeInSeconds)
        {
#if WALKIO_MOVE_FLOWFIELD_ASSIST
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

            var temporaryPointIndicationEntityArchetype = entityManager.CreateArchetype(
                typeof(GameMoveFlowFieldAssist.TemporaryPointIndication),
                typeof(GameMoveFlowFieldAssist.TemporaryPointIndicationProperty));

            var ffwd = flowFieldWorldData as GameMoveFlowField.Assist.Template.FlowFieldWorldData;
            var temporaryPointShowTime = (ffwd == null) ? timeInSeconds : ffwd.temporaryPointShowTime;

            points.ForEach(p =>
            {
                var temporaryPointIndicationEntity = entityManager.CreateEntity(temporaryPointIndicationEntityArchetype);
                entityManager.SetComponentData(temporaryPointIndicationEntity, new GameMoveFlowFieldAssist.TemporaryPointIndicationProperty
                {
                    GroupId = groupId,
                    Location = (float3)p,

                    IntervalMax = temporaryPointShowTime,
                    CountDown = 0
                });
            });
#endif
        }
    }
}
