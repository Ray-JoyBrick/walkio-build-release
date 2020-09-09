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
        private Scene _scene;

        //
        private EntityArchetype _entityArchetype;
        private EntityQuery _gridMapBlobAssetConstructedEventEntityQuery;

        private enum WaitingEvent
        {
            GridMapBlobAssetConstructed
        }

        //
        private readonly List<int> _waitingEventForLoadingDone = new List<int>
        {
            0,
            0
        };

        //
        // public GameCommand.ICommandService CommandService { get; set; }
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif

        public IGridWorldProvider GridWorldProvider { get; set; }

        public ILevelPropProvider LevelPropProvider { get; set; }
        public ILevelSelectionProvider LevelSelectionProvider { get; set; }

        //
        public string AtPart => "Stage";

        //
        public Template.LevelData LevelData { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"Module - Level - PrepareAssetSystem - Construct");

            RegisterToLoadFlow();
            RegisterToCleanupFlow();
        }

        protected override void OnCreate()
        {
            _logger.Debug($"Module - Level - PrepareAssetSystem - OnCreate");

            base.OnCreate();

            _entityArchetype = EntityManager.CreateArchetype(
                typeof(GridWorld),
                typeof(GridWorldProperty),
                typeof(GameFlowControl.StageUse));

            _gridMapBlobAssetConstructedEventEntityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<GridMapBlobAssetConstructed>()
                }
            });

            RequireForUpdate(_gridMapBlobAssetConstructedEventEntityQuery);
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
            var gridMapBlobAssetConstructedEventEntity =  _gridMapBlobAssetConstructedEventEntityQuery.GetSingletonEntity();

            _waitingEventForLoadingDone[(int)WaitingEvent.GridMapBlobAssetConstructed] = 1;

            //
            var allEventSet = CheckAllEventArrived(_waitingEventForLoadingDone);
            if (allEventSet)
            {
                // Delete event entity
                EntityManager.DestroyEntity(gridMapBlobAssetConstructedEventEntity);
                // Delete UniRx event

                // Reset list
                ResetWaitingEventSlots(_waitingEventForLoadingDone);

                // Notify
                FlowControl.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                {
                    Name = "Stage"
                });
            }
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
