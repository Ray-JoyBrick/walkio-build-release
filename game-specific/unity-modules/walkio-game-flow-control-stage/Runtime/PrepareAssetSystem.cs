namespace JoyBrick.Walkio.Game.FlowControl.Stage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

#if WALKIO_EXTENSION
    using GameExtension = JoyBrick.Walkio.Game.Extension;
#endif

    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;

    [GameFlowControl.DoneLoadingAssetWait("Stage")]
    [DisableAutoCreation]
    public partial class PrepareAssetSystem :
        SystemBase
        // GameCommon.ISystemContext
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(PrepareAssetSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private ScriptableObject _assetData;

        //
        public GameCommon.ISceneService SceneService { get; set; }
        public IFlowControl FlowControl { get; set; }

        public GameExtension.IExtensionService ExtensionService { get; set; }

        public string AtPart => "Stage";

        //
        public void Construct()
        {
            _logger.Debug($"Module - Flow Control - Stage - PrepareAssetSystem - Construct");

            //
            RegisterToLoadFlow();
            RegisterToCleanupFlow();
        }

        protected override void OnCreate()
        {
            _logger.Debug($"Module - Flow Control - Stage - PrepareAssetSystem - OnCreate");
            base.OnCreate();
        }

        protected override void OnUpdate() { }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable.Dispose();
        }
    }
}
