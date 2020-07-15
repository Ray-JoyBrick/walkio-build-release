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
    using GameCommand = JoyBrick.Walkio.Game.Command;

    //
    // [GameCommon.DoneLoadingAssetWait("Stage")]
    //
    [DisableAutoCreation]
    public class LoadAssetSystem :
        SystemBase,
        GameCommon.ISystemContext
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(LoadAssetSystem));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private ScriptableObject _levelSettingDataAsset;
        private SceneInstance _sceneInstance;

        //
        private EntityArchetype _entityArchetype;

        //
        public GameCommand.ICommandService CommandService { get; set; }
        public GameCommon.IFlowControl FlowControl { get; set; }
        
        //
        public string AtPart => "Stage";

        //
        public bool ProvideExternalAsset { get; set; }

        public ScriptableObject LevelSettingDataAsset
        {
            get => _levelSettingDataAsset;
            set => _levelSettingDataAsset = value;
        }

        public SceneInstance SceneInstance
        {
            get => _sceneInstance;
            set => _sceneInstance = value;
        }

        private async Task<(ScriptableObject, SceneInstance)> Load(string levelAssetName, string specificLevelName)
        {
            // What to load is defined below with async task
            var levelSettingAssetName = levelAssetName;
            var levelSettingAssetTask = GameCommon.Utility.AssetLoadingHelper.GetAsset<ScriptableObject>(levelSettingAssetName);

            var levelMainSceneAssetName = specificLevelName;
            var levelMainSceneAssetTask = GameCommon.Utility.AssetLoadingHelper.GetScene(levelMainSceneAssetName);
            
            var levelSettingAsset = await levelSettingAssetTask;
            var sceneInstance = await levelMainSceneAssetTask;

            return (levelSettingAsset, sceneInstance);
        }

        private void SetupGridMap(
            GameObject prefab,
            IList<Texture2D> texture2Ds, Vector2Int gridTileCount, 
            Vector2Int gridTileCellCount2D)
        {
            // var gridCells =
            //     Utility.GridHelper.GetGridObstacleIndicesFromTextures(texture2Ds, gridTileCount2D, gridTileCellCount2D);
            //
            // var authoring = prefab.GetComponent<GridMapBlobAssetAuthoring>();
            // if (authoring != null)
            // {
            //     authoring.gridCells = gridCells;
            //
            //     GameObject.Instantiate(authoring);
            // }
        }
        
        private void SetupAStarPathfinder(Scene scene, TextAsset textAsset)
        {
            _logger.Debug($"LoadAssetSystem - SetupAStarPathfinder - {scene.name}, {textAsset.bytes.Length}");
            
            AstarPath.active.data.DeserializeGraphs(textAsset.bytes);
        }

        private void MakeWorld(ScriptableObject levelSettingDataAsset, SceneInstance sceneInstance)
        {
            // var levelSettingData = levelSettingDataAsset as GameCommon.LevelSetting;

            // Should read the value from data asset instead of hard code
            var gridWorldTileCount = new Vector2Int(10, 10);
            var gridWorldTileCellCount = new Vector2Int(32, 32);
            var fakeTextAsset = new TextAsset();

            // SetupGridMap(gridWorldTileCount, gridWorldTileCellCount);
            SetupAStarPathfinder(sceneInstance.Scene, fakeTextAsset);
        }
        
        private void InternalLoadAsset(
            string levelAssetName, string specificLevelName,
            System.Action loadingDoneAction)
        {
            //
            Load(levelAssetName, specificLevelName).ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(result =>
                {
                    //
                    (_levelSettingDataAsset, _sceneInstance) = result;
                    
                    MakeWorld(_levelSettingDataAsset, _sceneInstance);
                    
                    loadingDoneAction();
                })
                .AddTo(_compositeDisposable);
        }        

        private void LoadingAsset(string levelAssetName, string specificLevelName)
        {
            if (ProvideExternalAsset)
            {
                // Asset is provided from somewhere else, just notify that the asset loading is done
                GameCommon.Utility.FlowControlHelper.NotifyFinishIndividualLoadingAsset(FlowControl, AtPart);
            }
            else
            {
                InternalLoadAsset(
                    levelAssetName, specificLevelName,
                    () =>
                    {
                        GameCommon.Utility.FlowControlHelper.NotifyFinishIndividualLoadingAsset(FlowControl, AtPart);
                    });
            }
        }

        //
        public void Construct()
        {
            _logger.Debug($"LoadAssetSystem - Construct");

            //
            FlowControl.LoadingAsset
                .Where(x => x.Name.Contains(AtPart))
                .Subscribe(x =>
                {
                    // Hard code here, should be given in event
                    var levelAssetName = $"Level Setting.asset";
                    var specificLevelName = $"Level 001";
                    LoadingAsset(levelAssetName, specificLevelName);
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnCreate()
        {
            _logger.Debug($"LoadAssetSystem - OnCreate");

            base.OnCreate();
        }

        protected override void OnUpdate() {}

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
