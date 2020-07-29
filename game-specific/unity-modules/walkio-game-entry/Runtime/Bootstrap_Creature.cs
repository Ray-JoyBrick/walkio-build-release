namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
    using System.Linq;
    // using HellTap.PoolKit;
    using Pathfinding;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    //
    using GameCreature = JoyBrick.Walkio.Game.Creature;
    using GameMoveFlowField = JoyBrick.Walkio.Game.Move.FlowField;

    public partial class Bootstrap
        : GameCreature.ICreatureProvider

    {
// #if ODIN_INSPECTOR
//         [Sirenix.OdinInspector.BoxGroup("Creature")]
// #endif
//         public ScriptableObject creatureData;
//
//         private GameCreature.Template.CreatureData CreatureData => creatureData as GameCreature.Template.CreatureData;

        private List<GameObject> _teamLeaderNpcPrefabs = new List<GameObject>();

        private int _addedToSceneTeamLeaderNpcCount = 0;

        public void AddTeamLeaderNpcPrefab(GameObject prefab)
        {
            _logger.Debug($"Bootstrap - AddTeamLeaderNpcPrefab");
            var contained = _teamLeaderNpcPrefabs.Contains(prefab);
            if (!contained)
            {
                _teamLeaderNpcPrefabs.Add(prefab);
            }
        }

        //
        public void CreateTeamLeaderNpcAt(Vector3 location)
        {
            _logger.Debug($"Bootstrap - CreateTeamLeaderNpcAt");

            var maxCount = _teamLeaderNpcPrefabs.Count;
            var randomIndex = UnityEngine.Random.Range(0, maxCount);
            var prefab = _teamLeaderNpcPrefabs[randomIndex];

            var flowFieldMoveAuthoring = prefab.GetComponent<GameMoveFlowField.FlowFieldMoveAuthoring>();
            if (flowFieldMoveAuthoring != null)
            {
                flowFieldMoveAuthoring.belongToGroup = _addedToSceneTeamLeaderNpcCount + 1;
            }
            var createdInstance = GameObject.Instantiate(prefab, location, quaternion.identity);

            ++_addedToSceneTeamLeaderNpcCount;

            MoveToCurrentScene(createdInstance);
        }

        public void CreateTeamLeaderPlayerAt(Vector3 location)
        {

        }
    }
}
