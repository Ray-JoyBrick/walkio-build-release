namespace JoyBrick.Walkio.Game.Battle
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;
    
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCommand = JoyBrick.Walkio.Game.Command;
    using GameExtension = JoyBrick.Walkio.Game.Extension;

    [DisableAutoCreation]
    public class LoadBattleSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(LoadBattleSystem));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private GameObject _battleUsePoolPrefab;
        private GameObject _teamForceSetPrefab;
        private GameObject _teamForceUnitPrefab;
        private GameObject _neutralForceUnitPrefab;

        //
        private GameObject _battleUsePool;

        //
        public GameObject RefBootstrap { get; set; }
        public GameCommand.ICommandService CommandService { get; set; }
        // public GameCommand.IInfoPresenter InfoPresenter { get; set; }
        public GameCommon.IFlowControl FlowControl { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"LoadBattleSystem - Construct");
            
            base.OnCreate();
            
            //
            FlowControl.LoadingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    LoadingAsset();
                })
                .AddTo(_compositeDisposable);            

            // //
            // CommandService.CommandStream
            //     .Do(x => _logger.Debug($"Receive Command Stream: {x}"))
            //     .Where(x => (x as GameCommand.ActivateLoadingViewCommand) != null)
            //     .Subscribe(x =>
            //     {
            //         _logger.Debug($"LoadAppHudSystem - Construct - Receive ActivateLoadingViewCommand");
            //         var activateLoadingViewCommand = (x as GameCommand.ActivateLoadingViewCommand);
            //         //
            //         ActivateLoadingView(activateLoadingViewCommand.Flag);
            //     })
            //     .AddTo(_compositeDisposable);
        }

        private void LoadingAsset()
        {
            //
            Load().ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(result =>
                {
                    //
                    (_battleUsePoolPrefab, _teamForceSetPrefab, _teamForceUnitPrefab, _neutralForceUnitPrefab) = result;
                            
                    //
                    _battleUsePool = GameObject.Instantiate(_battleUsePoolPrefab);
                    var scene = SceneManager.GetSceneByName("Entry");
                    if (scene.IsValid())
                    {
                        SceneManager.MoveGameObjectToScene(_battleUsePool, scene);
                    }
                    
                    // AddCommandStreamAndInfoStream(_canvas);
                            
                    //
                    FlowControl.FinishLoadingAsset(new GameCommon.FlowControlContext
                    {
                        Name = "Stage"
                    });
                })
                .AddTo(_compositeDisposable);            
        }

        private async Task<T> GetAsset<T>(string addressName)
        {
            var handle = Addressables.LoadAssetAsync<T>(addressName);
            var r = await handle.Task;
        
            return r;
        }
        
        private async Task<(GameObject, GameObject, GameObject, GameObject)> Load()
        {
            var battleUsePoolPrefabTask = GetAsset<GameObject>($"Battle Use Pool");
            var teamForceSetPrefabTask = GetAsset<GameObject>($"Team Force Set");
            var teamForceUnitPrefabTask = GetAsset<GameObject>($"Team Force Unit");
            var neutralForceUnitPrefabTask = GetAsset<GameObject>($"Neutral Force Unit");

            var (battleUsePoolPrefab, teamForceSetPrefab, teamForceUnitPrefab, neutralForceUnitPrefab) =
                (await battleUsePoolPrefabTask, await teamForceSetPrefabTask, await teamForceUnitPrefabTask, await neutralForceUnitPrefabTask);

            return (battleUsePoolPrefab, teamForceSetPrefab, teamForceUnitPrefab, neutralForceUnitPrefab);
        }
        
        protected override void OnUpdate() {}

        public void RemovingAssets()
        {
            //
            if (_battleUsePoolPrefab != null)
            {
                Addressables.ReleaseInstance(_battleUsePoolPrefab);
            }
            
            // if (_viewLoadingPrefab != null)
            // {
            //     Addressables.ReleaseInstance(_viewLoadingPrefab);
            // }
            //
            // if (_timelineAsset != null)
            // {
            //     Addressables.Release(_timelineAsset);
            // }
            //
            // if (_i2Asset != null)
            // {
            //     Addressables.Release(_i2Asset);
            // }
            
            //
            if (_battleUsePool != null)
            {
                GameObject.Destroy(_battleUsePool);
            }            
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            RemovingAssets();
            
            _compositeDisposable?.Dispose();
        }
    }
}
