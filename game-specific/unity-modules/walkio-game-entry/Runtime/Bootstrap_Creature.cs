namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
    using System.Linq;
    // using HellTap.PoolKit;
    using Pathfinding;
    using SickscoreGames.HUDNavigationSystem;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    //
    using GameCreature = JoyBrick.Walkio.Game.Creature;
    using GameMoveFlowField = JoyBrick.Walkio.Game.Move.FlowField;

    public partial class Bootstrap
        : GameCreature.ICreatureProvider,
            GameCreature.ICreatureOverviewProvider

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
        public void CreateTeamLeaderNpcAt(int id, Vector3 location)
        {
            _logger.Debug($"Bootstrap - CreateTeamLeaderNpcAt - location: {location}");

            var maxCount = _teamLeaderNpcPrefabs.Count;
            var randomIndex = UnityEngine.Random.Range(0, maxCount);
            var prefab = _teamLeaderNpcPrefabs[randomIndex];

            // Need to change components on prefab before being instantiated
            var flowFieldMoveAuthoring = prefab.GetComponent<GameMoveFlowField.FlowFieldMoveAuthoring>();
            if (flowFieldMoveAuthoring != null)
            {
                flowFieldMoveAuthoring.belongToGroup = _addedToSceneTeamLeaderNpcCount + 1;
            }

            var teamForceAuthoring = prefab.GetComponent<GameCreature.TeamForceAuthoring>();
            if (teamForceAuthoring != null)
            {
                teamForceAuthoring.teamId = id;
            }


            var createdInstance = GameObject.Instantiate(prefab, location, quaternion.identity);
            
            // Need to change components on instance

            _createdTeamLeaderNpcs.Add(createdInstance);

            ++_addedToSceneTeamLeaderNpcCount;

            MoveToLevelAtScene(createdInstance);
        }

        public void CreateTeamLeaderPlayerAt(int id, Vector3 location)
        {
            _logger.Debug($"Bootstrap - CreateTeamLeaderPlayerAt - location: {location}");

            // Just create random player for now, will pass exact creation index
            var maxCount = _teamLeaderPlayerPrefabs.Count;
            var randomIndex = UnityEngine.Random.Range(0, maxCount);
            var prefab = _teamLeaderPlayerPrefabs[randomIndex];

            var teamForceAuthoring = prefab.GetComponent<GameCreature.TeamForceAuthoring>();
            if (teamForceAuthoring != null)
            {
                teamForceAuthoring.teamId = id;
            }
            
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
        
        //
        private readonly List<GameCreature.Template.TeamLeaderOverview> _teamLeaderOverviews =
            new List<GameCreature.Template.TeamLeaderOverview>();
        private readonly List<GameCreature.Template.TeamMinionOverview> _teamMinionOverviews =
            new List<GameCreature.Template.TeamMinionOverview>();

        public List<GameCreature.Template.TeamLeaderOverview> TeamLeaderOverviews => _teamLeaderOverviews;
        public List<GameCreature.Template.TeamMinionOverview> TeamMinionOverviews => _teamMinionOverviews;
        
        public void AddTeamLeaderOverview(GameCreature.Template.TeamLeaderOverview teamLeaderOverview)
        {
            _teamLeaderOverviews.Add(teamLeaderOverview);
        }

        public void AddTeamMinionOverview(GameCreature.Template.TeamMinionOverview teamMinionOverview)
        {
            _teamMinionOverviews.Add(teamMinionOverview);
        }
        
        // Make this loaded from a data asset
        private List<Color> _groupColors = new List<Color>
        {
            Color.blue,
            Color.cyan,
            Color.green,
            Color.magenta,
            Color.red,
            Color.yellow,
            Color.gray
        };

        public void SetupNavigationHudIndication()
        {
            var index = 0;
            _createdTeamLeaderPlayers.Concat(_createdTeamLeaderNpcs).ToList()
                .ForEach(x =>
                {
                    var color = _groupColors[index];
                    ++index;
                    var hudNavigationElement = x.GetComponent<HUDNavigationElement>();
                    if (hudNavigationElement != null)
                    {
                        _logger.Debug($"Bootstrap - CreateTeamLeaderNpcAt - hudNavigationElement not null");
                        if (hudNavigationElement.Indicator != null)
                        {
                            _logger.Debug($"Bootstrap - CreateTeamLeaderNpcAt - hudNavigationElement indicator not null");
                            if (hudNavigationElement.Indicator.OffscreenIcon != null)
                            {
                                _logger.Debug($"Bootstrap - CreateTeamLeaderNpcAt - hudNavigationElement indicator off not null");
                                hudNavigationElement.Indicator.OffscreenIcon.color = color;
                            }

                            if (hudNavigationElement.Indicator.OnscreenIcon != null)
                            {
                                _logger.Debug($"Bootstrap - CreateTeamLeaderNpcAt - hudNavigationElement indicator on not null");
                                hudNavigationElement.Indicator.OnscreenIcon.color = color;
                            }
                        }
                    }
                });
        }
    }
}
