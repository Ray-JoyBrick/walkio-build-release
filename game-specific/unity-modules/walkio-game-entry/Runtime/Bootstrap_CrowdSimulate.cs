namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
    using System.Linq;
    using HellTap.PoolKit;
    using Pathfinding;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    //
    using GameMoveCrowdSimulate = JoyBrick.Walkio.Game.Move.CrowdSimulate;

    public partial class Bootstrap

#if WALKIO_MOVE_CROWDSIMULATE
        : GameMoveCrowdSimulate.ICrowdSimulateWorldProvider
#endif

    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Flow Field")]
#endif
        public ScriptableObject crowdSimulateWorldData;

        public ScriptableObject CrowdSimulateWorldData => crowdSimulateWorldData;

        public Entity CrowdSimulateWorldEntity { get; set; }

        private void SetupCrowdSimulatePart()
        {
            // _pathPointFoundEventEntityArchetype = _entityManager.CreateArchetype(
            //     typeof(GameMoveFlowField.PathPointFound),
            //     typeof(GameMoveFlowField.PathPointFoundProperty),
            //     typeof(GameMoveFlowField.PathPointSeparationBuffer),
            //     typeof(GameMoveFlowField.PathPointBuffer));
        }
    }
}
