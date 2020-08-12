namespace JoyBrick.Walkio.Game.Creature
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

#if WALKIO_FLOWCONTROL
    [GameFlowControl.DoneLoadingAssetWait("App")]
#endif
    [DisableAutoCreation]
    public partial class PrepareOverviewAssetSystem :
        SystemBase
        // GameCommon.ISystemContext
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(PrepareOverviewAssetSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private ScriptableObject _creatureOverviewRepoDataAsset;

        //
        public ICreatureProvider CreatureProvider { get; set; }
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif

        //
        public string AtPart => "App";

        //
        public void Construct()
        {
            _logger.Debug($"Module - Creature - PrepareOverviewAssetSystem - Construct");

            RegisterToLoadFlow();
            RegisterToCleanupFlow();
        }

        protected override void OnCreate()
        {
            _logger.Debug($"Module - Creature - PrepareOverviewAssetSystem - OnCreate");

            base.OnCreate();
        }

        protected override void OnUpdate()
        {
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
