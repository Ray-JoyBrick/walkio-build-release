namespace JoyBrick.Walkio.Game.Level
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Pathfinding;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCommand = JoyBrick.Walkio.Game.Command;

    //
    [GameCommon.DoneLoadingAssetWait("Stage")]
    //
    [DisableAutoCreation]
    public class LoadLevelSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(LoadLevelSystem));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private ScriptableObject _waypointDataAsset;
        private GameObject _waypointPathBlobAssetAuthoringPrefab;
        
        //
        private EntityArchetype _entityArchetype;

        //
        public GameCommand.ICommandService CommandService { get; set; }
        public GameCommon.IFlowControl FlowControl { get; set; }
        
        //
        public bool ProvideExternalAsset { get; set; }

        public ScriptableObject WaypointDataAsset
        {
            get => _waypointDataAsset;
            set => _waypointDataAsset = value;
        }

        public GameObject WaypointPathBlobAssetAuthoringPrefab
        {
            get => _waypointPathBlobAssetAuthoringPrefab;
            set => _waypointPathBlobAssetAuthoringPrefab = value;
        }

        //
        public void Construct()
        {
            _logger.Debug($"LoadLevelSystem - Construct");

            base.OnCreate();

            //
            FlowControl.LoadingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    LoadingAsset();
                })
                .AddTo(_compositeDisposable);
        }

        private void LoadingAsset()
        {
            if (ProvideExternalAsset)
            {
                FlowControl.FinishLoadingAsset(new GameCommon.FlowControlContext
                {
                    Name = "Stage"
                });
            }
            else
            {
                //
                Load().ToObservable()
                    .ObserveOnMainThread()
                    .SubscribeOnMainThread()
                    .Subscribe(result =>
                    {
                        FlowControl.FinishLoadingAsset(new GameCommon.FlowControlContext
                        {
                            Name = "Stage"
                        });
                    })
                    .AddTo(_compositeDisposable);
            }
        }

        private async Task Load()
        {
            await Task.Delay(System.TimeSpan.FromMilliseconds(2000));
        }

        protected override void OnCreate()
        {
            _logger.Debug($"LoadLevelSystem - OnCreate");

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
