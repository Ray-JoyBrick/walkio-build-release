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
        private ScriptableObject _levelDataAsset;

        //
        // public GameCommand.ICommandService CommandService { get; set; }
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif

        //
        public string AtPart => "App";

        //
        public Template.LevelData LevelData { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"Module - Level - PrepareOverviewAssetSystem - Construct");

            RegisterToLoadFlow();
            RegisterToCleanupFlow();
        }

        protected override void OnCreate()
        {
            _logger.Debug($"Module - Level - PrepareOverviewAssetSystem - OnCreate");

            base.OnCreate();
        }

        private static bool CheckAllEventArrived(List<int> eventSlots)
        {
            var notSetEvents = eventSlots.Where(x => x == 0);

            // return !notSetEvents.Any();
            return true;
        }

        private static void ResetWaitingEventSlots(List<int> eventSlots)
        {
            for (var i = 0; i < eventSlots.Count; ++i)
            {
                eventSlots[i] = 0;
            }
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
