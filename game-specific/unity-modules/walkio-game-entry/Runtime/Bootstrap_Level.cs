namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
    using System.Linq;
    using Hud.Stage;
    using Opsive.UltimateCharacterController.Camera;
    // using HellTap.PoolKit;
    using Pathfinding;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    //
    using GameCommand = JoyBrick.Walkio.Game.Command;
#if WALKIO_LEVEL
    using GameLevel = JoyBrick.Walkio.Game.Level;
#endif

    public partial class Bootstrap

#if WALKIO_LEVEL
        : GameLevel.IGridWorldProvider,
            GameLevel.ILevelPropProvider
#endif

    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Level")]
#endif
        public ScriptableObject gridWorldData;

        public ScriptableObject GridWorldData => gridWorldData;

        private void SetupLevelPart()
        {
            _logger.Debug($"Bootstrap - SetupLevelPart");

            CommandStream
                .Subscribe(x =>
                {
                    _logger.Debug($"Bootstrap - SetupLevelPart - {x}");

                    if (x is GameCommand.ShowHideSceneObjectCommand showHideSceneObjectCommand)
                    {
                        var archetype =
                            World.DefaultGameObjectInjectionWorld.EntityManager.CreateArchetype(
                                typeof(GameLevel.Assist.ShowHideRequest),
                                typeof(GameLevel.Assist.ShowHideRequestProperty));

                        var eventEntity =
                            World.DefaultGameObjectInjectionWorld.EntityManager.CreateEntity(archetype);
                        var category = showHideSceneObjectCommand.Category;
                        var hide = showHideSceneObjectCommand.Hide;
                        World.DefaultGameObjectInjectionWorld.EntityManager.SetComponentData(
                            eventEntity, new GameLevel.Assist.ShowHideRequestProperty
                            {
                                Category = category,
                                Hide = hide
                            });
                    }
                })
                .AddTo(_compositeDisposable);

            TeamForceUnitCounts
                .ObserveAdd()
                .Subscribe(x =>
                {
                    // _logger.Debug($"Bootstrap - SetupLevelPart - TeamForceUnitCounts add");
                    _notifyInfo.OnNext(new TeamUnitCountInfo
                    {
                        TeamId = x.Key,
                        Count = x.Value
                    });
                })
                .AddTo(_compositeDisposable);

            TeamForceUnitCounts
                .ObserveReplace()
                .Subscribe(x =>
                {
                    // _logger.Debug($"Bootstrap - SetupLevelPart - TeamForceUnitCounts replace");
                    _notifyInfo.OnNext(new TeamUnitCountInfo
                    {
                        TeamId = x.Key,
                        Count = x.NewValue
                    });
                })
                .AddTo(_compositeDisposable);
        }

        public Scene LevelAtScene { get; set; }

        public Camera LevelCamera { get; set; }

        public GameObject MainPlayerVirtualCamera { get; set; }

        public void SetupFollowingCamera(GameObject playerGo)
        {
            var cameraController = LevelCamera.GetComponent<CameraController>();
            cameraController.Character = playerGo;
            // cameraController.
        }

        //
        private ReactiveDictionary<int, int> _teamForceUnitCounts = new ReactiveDictionary<int, int>();
        public ReactiveDictionary<int, int> TeamForceUnitCounts => _teamForceUnitCounts;

        //
        private void MoveToLevelAtScene(GameObject inGo)
        {
            if (LevelAtScene.IsValid())
            {
                SceneManager.MoveGameObjectToScene(inGo, LevelAtScene);
            }
            else
            {
                _logger.Warning($"Bootstrap - MoveToLevelAtScene - LevelAtScene is not valid");
            }
        }
    }
}
