namespace JoyBrick.Walkio.Game.Placeholder
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

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameCommand = JoyBrick.Walkio.Game.Command;

    //
    [GameCommon.DoneLoadingAssetWait("Stage")]
    //
    [DisableAutoCreation]
    public class EmptyStageLoadingSystem : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(EmptyStageLoadingSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        public GameCommon.IFlowControl FlowControl { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"EmptyStageLoadingSystem - Construct");
            
            //
            FlowControl.LoadingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"EmptyStageLoadingSystem - Construct - Receive LoadingAsset signal");

                    LoadingAsset();
                })
                .AddTo(_compositeDisposable);
        }
        
        private void LoadingAsset()
        {
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

        private async Task Load()
        {
            // Wait some time before signaling finish loading asset
            await Task.Delay(System.TimeSpan.FromMilliseconds(2000));
        }

        protected override void OnUpdate() {}

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
