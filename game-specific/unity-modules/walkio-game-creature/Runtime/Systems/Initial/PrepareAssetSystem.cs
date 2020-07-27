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
    [GameFlowControl.DoneLoadingAssetWait("Stage")]
#endif
    [DisableAutoCreation]
    public partial class PrepareAssetSystem :
        SystemBase
        // GameCommon.ISystemContext
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(PrepareAssetSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private ScriptableObject _levelDataAsset;
        private SceneInstance _sceneInstance;

        //
        private EntityArchetype _entityArchetype;
        private EntityQuery _gridMapBlobAssetConstructedEventEntityQuery;

        // private enum WaitingEvent
        // {
        //     GridMapBlobAssetConstructed
        // }
        //
        // //
        // private readonly List<int> _waitingEventForLoadingDone = new List<int>
        // {
        //     0,
        //     0
        // };

        //
        // public GameCommand.ICommandService CommandService { get; set; }
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif

        //
        public string AtPart => "Stage";

        //
        public void Construct()
        {
            _logger.Debug($"Module - Creature - PrepareAssetSystem - Construct");

            RegisterToLoadFlow();
            RegisterToCleanupFlow();
        }

        protected override void OnCreate()
        {
            _logger.Debug($"Module - Creature - PrepareAssetSystem - OnCreate");

            base.OnCreate();
        }

        private static bool CheckAllEventArrived(List<int> eventSlots)
        {
            var notSetEvents = eventSlots.Where(x => x == 0);

            return !notSetEvents.Any();
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
            // var gridMapBlobAssetConstructedEventEntity =  _gridMapBlobAssetConstructedEventEntityQuery.GetSingletonEntity();

            // _waitingEventForLoadingDone[(int)WaitingEvent.GridMapBlobAssetConstructed] = 1;

            //
            // var allEventSet = CheckAllEventArrived(_waitingEventForLoadingDone);
            // if (allEventSet)
            // {
            //     // Delete event entity
            //     EntityManager.DestroyEntity(gridMapBlobAssetConstructedEventEntity);
            //     // Delete UniRx event
            //
            //     // Reset list
            //     ResetWaitingEventSlots(_waitingEventForLoadingDone);
            //
            //     // Notify
            //     FlowControl.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
            //     {
            //         Name = "Stage"
            //     });
            // }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
