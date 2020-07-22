namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using System.Linq;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;

    //
#if WALKIO_LEVEL
    using GameLevel = JoyBrick.Walkio.Game.Level;
#endif

#if WALKIO_FLOWFIELD || WALKIO_WAYPOINT || WALKIO_CROWDSIM
    using GameMove = JoyBrick.Walkio.Game.Move;
#endif

    public partial class Bootstrap
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Post Assistable Ecs Setup")]
#endif
        public GameObject flowFieldTileBlobAssetAuthoringPrefab;
        
        public void HandleAfterEcsSetup()
        {
            var entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
            // Provide the testing use asset to LoadLevelSystem
#if WALKIO_LEVEL
            var loadLevelSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameLevel.LoadLevelSystem>();
            loadLevelSystem.ProvideExternalAsset = true;

            loadLevelSystem.LevelSettingDataAsset = levelSettingData;
            loadLevelSystem.SceneInstance = sceneInstance;

            var gridWorldEntityArchetype = entityManager.CreateArchetype(
                typeof(GameLevel.Common.GridWorld),
                typeof(GameLevel.Common.GridWorldProperty));

            var gridWorldEntity = entityManager.CreateEntity(gridWorldEntityArchetype);
            entityManager.SetComponentData(gridWorldEntity, new GameLevel.Common.GridWorldProperty
            {
                GridTileCount = new int2(4, 4),
                GridTileCellCount = new int2(8, 8)
            });

            var gridWorldProvider = _assistable.RefGameObject.GetComponent<GameLevel.Common.IGridWorldProvider>();
            if (gridWorldProvider != null)
            {
                gridWorldProvider.GridWorldEntity = gridWorldEntity;
            }
#endif

            // Provide the testing use asset to LoadFlowFieldSystem
#if WALKIO_FLOWFIELD
            var flowFieldLoadAssetSystem = World.DefaultGameObjectInjectionWorld.GetExistingSystem<GameMove.FlowField.LoadAssetSystem>();
            flowFieldLoadAssetSystem.ProvideExternalAsset = true;

            flowFieldLoadAssetSystem.SettingDataAsset = flowFieldSettingData;
            
            var flowFieldWorldEntityArchetype = entityManager.CreateArchetype(
                typeof(GameMove.FlowField.Common.FlowFieldWorld),
                typeof(GameMove.FlowField.Common.FlowFieldWorldProperty));
            
            var flowFieldWorldEntity = entityManager.CreateEntity(flowFieldWorldEntityArchetype);
            entityManager.SetComponentData(flowFieldWorldEntity, new GameMove.FlowField.Common.FlowFieldWorldProperty
            {
                TileCount = new int2(4, 4),
                TileCellCount = new int2(8, 8)
            });
            
            var flowFieldWorldProvider = _assistable.RefGameObject.GetComponent<GameMove.FlowField.Common.IFlowFieldWorldProvider>();
            if (flowFieldWorldProvider != null)
            {
                _logger.Debug($"Bootstrap Assist - HandleAfterEcsSetup - Setting flow filed world provider");
                flowFieldWorldProvider.FlowFieldWorldEntity = flowFieldWorldEntity;
                flowFieldWorldProvider.FlowFieldTileBlobAssetAuthoringPrefab = flowFieldTileBlobAssetAuthoringPrefab;
            }
#endif
        }
    }
}
