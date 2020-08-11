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

        // This is read from Addressable and store here for later use as a cache
        private List<GameObject> _teamLeaderNpcPrefabs = new List<GameObject>();
        private List<GameObject> _teamLeaderPlayerPrefabs = new List<GameObject>();
        private List<GameObject> _teamMinionPrefabs = new List<GameObject>();

        private List<GameCreature.MinionData> _teamMinionDatas = new List<GameCreature.MinionData>();

        private int _addedToSceneTeamLeaderNpcCount = 0;

        //
        private readonly List<GameObject> _createdTeamLeaderNpcs = new List<GameObject>();
        private readonly List<GameObject> _createdTeamLeaderPlayers = new List<GameObject>();

        private void SetupCreaturePart()
        {
            _logger.Debug($"Bootstrap - SetupCreaturePart");

            AssetUnloadingStarted
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"Bootstrap - SetupCreaturePart - receive AssetUnloadingStarted");

                    _addedToSceneTeamLeaderNpcCount = 0;

                    _teamLeaderNpcPrefabs.Clear();
                    _teamLeaderPlayerPrefabs.Clear();
                    _teamMinionPrefabs.Clear();
                    _teamMinionDatas.Clear();

                    _createdTeamLeaderNpcs.ForEach(y =>
                    {
                        GameObject.Destroy(y);
                    });
                    _createdTeamLeaderNpcs.Clear();

                    _createdTeamLeaderPlayers.ForEach(y =>
                    {
                        GameObject.Destroy(y);
                    });
                    _createdTeamLeaderPlayers.Clear();
                })
                .AddTo(_compositeDisposable);
        }

        public void AddTeamLeaderNpcPrefab(GameObject prefab)
        {
            _logger.Debug($"Bootstrap - AddTeamLeaderNpcPrefab");
            var contained = _teamLeaderNpcPrefabs.Contains(prefab);
            if (!contained)
            {
                _teamLeaderNpcPrefabs.Add(prefab);
            }
        }

        public void AddTeamLeaderPlayerPrefab(GameObject prefab)
        {
            _logger.Debug($"Bootstrap - AddTeamLeaderPlayerPrefab");
            var contained = _teamLeaderPlayerPrefabs.Contains(prefab);
            if (!contained)
            {
                _teamLeaderPlayerPrefabs.Add(prefab);
            }
        }

        public void AddTeamMinionPrefab(GameObject prefab)
        {
            _logger.Debug($"Bootstrap - AddTeamMinionPrefab");
            var contained = _teamMinionPrefabs.Contains(prefab);
            if (!contained)
            {
                _teamMinionPrefabs.Add(prefab);
            }
        }

        public void AddTeamMinionData(GameCreature.MinionData minionData)
        {
            _logger.Debug($"Bootstrap - AddTeamMinionData");
            var contained = _teamMinionDatas.Contains(minionData);
            if (!contained)
            {
                _teamMinionDatas.Add(minionData);
            }
        }

        //
        public void CreateTeamLeaderNpcAt(Vector3 location)
        {
            _logger.Debug($"Bootstrap - CreateTeamLeaderNpcAt - location: {location}");

            var maxCount = _teamLeaderNpcPrefabs.Count;
            var randomIndex = UnityEngine.Random.Range(0, maxCount);
            var prefab = _teamLeaderNpcPrefabs[randomIndex];

            var flowFieldMoveAuthoring = prefab.GetComponent<GameMoveFlowField.FlowFieldMoveAuthoring>();
            if (flowFieldMoveAuthoring != null)
            {
                flowFieldMoveAuthoring.belongToGroup = _addedToSceneTeamLeaderNpcCount + 1;
            }
            var createdInstance = GameObject.Instantiate(prefab, location, quaternion.identity);

            _createdTeamLeaderNpcs.Add(createdInstance);

            ++_addedToSceneTeamLeaderNpcCount;

            MoveToLevelAtScene(createdInstance);
        }

        public void CreateTeamLeaderPlayerAt(Vector3 location)
        {
            _logger.Debug($"Bootstrap - CreateTeamLeaderPlayerAt - location: {location}");

            // Just create random player for now, will pass exact creation index
            var maxCount = _teamLeaderPlayerPrefabs.Count;
            var randomIndex = UnityEngine.Random.Range(0, maxCount);
            var prefab = _teamLeaderPlayerPrefabs[randomIndex];

            // Need to pass this created instance so that level setup, such as camera has the
            // target to follow
            var createdInstance = GameObject.Instantiate(prefab, location, quaternion.identity);

            _createdTeamLeaderPlayers.Add(createdInstance);

            MoveToLevelAtScene(createdInstance);
        }

        public GameObject GetTeamMinionPrefab(int index)
        {
            return _teamMinionPrefabs[index];
        }

        public int GetMinionDataCount => _teamMinionDatas.Count;

        public GameCreature.MinionData GetMinionDataByIndex(int index)
        {
            return _teamMinionDatas[index];
        }

        public GameObject GetCurrentPlayer()
        {
            // Just get the first from the list
            if (!_createdTeamLeaderPlayers.Any())
            {
                return null;
            }

            return _createdTeamLeaderPlayers.First();
        }
    }
}
