namespace JoyBrick.Walkio.Game.Level
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Pathfinding;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    // using GameCommand = JoyBrick.Walkio.Game.Command;

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

    public partial class PrepareAssetSystem
        // GameCommon.ISystemContext
    {
        //
        public bool ProvideExternalAsset { get; set; }

        private async Task<(ScriptableObject, SceneInstance)> Load(string levelAssetName, string specificLevelName)
        //private async Task<ScriptableObject> Load(string levelAssetName, string specificLevelName)
        {
            // What to load is defined below with async task
            var levelDataAssetName = levelAssetName;
            var levelDataAssetTask = GameCommon.Utility.AssetLoadingHelper.GetAsset<ScriptableObject>(levelDataAssetName);

            var levelMainSceneAssetName = specificLevelName;
            var levelMainSceneAssetTask = GameCommon.Utility.AssetLoadingHelper.GetScene(levelMainSceneAssetName);
            //
            var levelSettingAsset = await levelDataAssetTask;
            var sceneInstance = await levelMainSceneAssetTask;

            return (levelSettingAsset, sceneInstance);
            // return levelSettingAsset;
        }

        private void SetupGridMap(
            GameObject prefab,
            Dictionary<Color32, int> lookupTable,
            IList<Texture2D> texture2Ds, Vector2Int gridTileCount2D,
            Vector2Int gridTileCellCount2D, Vector2 gridWorldCellSize)
        {
            var gridCells =
                Utility.GridWorldHelper.GetGridObstacleIndicesFromTextures(lookupTable, texture2Ds, gridTileCount2D, gridTileCellCount2D);

            var authoring = prefab.GetComponent<GridMapBlobAssetAuthoring>();
            if (authoring != null)
            {
                authoring.gridCellCount = gridTileCount2D * gridTileCellCount2D;
                authoring.gridCellSize = gridWorldCellSize;
                authoring.gridCells = gridCells;

                 _logger.Debug($"Module - Level - LoadAssetSystem - SetupGridMap - grid cells assigned to authoring prefab");

                 // This may takes several secs to get ready
                GameObject.Instantiate(authoring);
            }
        }

        // private void SetupAStarPathfinder(Scene scene, TextAsset textAsset)
        // {
        //     _logger.Debug($"LoadAssetSystem - SetupAStarPathfinder - {scene.name}, {textAsset.bytes.Length}");
        //
        //     AstarPath.active.data.DeserializeGraphs(textAsset.bytes);
        // }

        private void MakeWorld(ScriptableObject levelSettingDataAsset, SceneInstance sceneInstance)
        {
            //
            var gridWorldEntity = EntityManager.CreateEntity(_entityArchetype);

#if UNITY_EDITOR
            EntityManager.SetName(gridWorldEntity, $"Grid World");
#endif
            //
            _logger.Debug($"Module - Level - LoadAssetSystem - MakeWorld - load level data");
            var levelData = levelSettingDataAsset as Template.LevelData;
            if (levelData != null)
            {
                // Should read the value from data asset instead of hard code
                var gridWorldTileCount = levelData.gridWorldTileCount;
                var gridWorldTileCellCount = levelData.gridWorldTielCellCount;
                var gridWorldCellSize = levelData.gridWorldCellSize;
                var gridWorldData = GridWorldProvider.GridWorldData as Template.GridWorldData;

                var lookupTable =
                    levelData.areaLookup
                        .ToDictionary(x => x.areaColor, x => x.index);

                var prefab = gridWorldData.gridMapBlobAssetAuthoringPrefab;

                SetupGridMap(
                    prefab,
                    lookupTable, levelData.subLevelImages,
                    gridWorldTileCount, gridWorldTileCellCount, gridWorldCellSize);
            }

            // var fakeTextAsset = new TextAsset();
            // // SetupGridMap(gridWorldTileCount, gridWorldTileCellCount);
            // SetupAStarPathfinder(sceneInstance.Scene, fakeTextAsset);
        }

        private void InternalLoadAsset(
            string levelAssetName, string specificLevelName,
            System.Action loadingDoneAction)
        {
            Load(levelAssetName, specificLevelName).ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(result =>
                {
                    //
                    (_levelDataAsset, _sceneInstance) = result;
                    // _levelDataAsset = result;

                    MakeWorld(_levelDataAsset, _sceneInstance);

                    loadingDoneAction();
                })
                .AddTo(_compositeDisposable);
        }

        private void LoadingAsset(string levelAssetName, string specificLevelName)
        {
            if (ProvideExternalAsset)
            {
                // Asset is provided from somewhere else, just notify that the asset loading is done
#if WALKIO_FLOWCONTROL
                FlowControl.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                {
                    Name = "Stage"
                });
#endif
            }
            else
            {
                InternalLoadAsset(
                    levelAssetName, specificLevelName,
                    () =>
                    {
                        // Since internal loading might be very time consuming, after it is finished, it will
                        // send an event entity. This event entity is caught in Update and process further.

                        // FlowControl.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                        // {
                        //     Name = "Stage"
                        // });
                    });
            }
        }

        //
        private void RegisterToLoadFlow()
        {
            _logger.Debug($"Module - Level - PrepareAssetSystem - RegisterToLoadFlow");

#if WALKIO_FLOWCONTROL
            //
            FlowControl?.AssetLoadingStarted
                .Where(x => x.Name.Contains(AtPart))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Level - LoadAssetSystem - Construct - Receive AssetLoadingStarted");

                    // Hard code here, should be given in event
                    // var levelAssetName = $"Level Setting.asset";
                    // var specificLevelName = $"Level 001";
                    var levelAssetName = $"Level 001/Level Data";
                    var specificLevelName = $"Level 001/Main";
                    LoadingAsset(levelAssetName, specificLevelName);
                })
                .AddTo(_compositeDisposable);
#endif
        }

    }
}
