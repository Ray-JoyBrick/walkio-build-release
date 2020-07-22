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
    [GameCommon.DoneSettingAssetWait("Stage")]
    //
    [DisableAutoCreation]
    public class EmptyStageSettingSystem : SystemBase
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(EmptyStageSettingSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        public GameCommon.IFlowControl FlowControl { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"EmptyStageSettingSystem - Construct");
            
            FlowControl.SettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"EmptyStageSettingSystem - Construct - Receive SettingAsset");
                    
                    FlowControl.FinishSetting(new GameCommon.FlowControlContext
                    {
                        Name = "Stage"
                    });
                })
                .AddTo(_compositeDisposable); 
        }
        
        protected override void OnUpdate() {}

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
