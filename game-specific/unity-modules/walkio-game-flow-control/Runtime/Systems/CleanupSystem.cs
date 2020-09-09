namespace JoyBrick.Walkio.Game.FlowControl
{
    using System;
    using Command;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;

    //
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;

    [DisableAutoCreation]
    public class CleanupSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(LoadingDoneCheckSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private EntityArchetype _removeStageUseEventEntityArcehtype;

        public GameFlowControl.IFlowControl FlowControl { get; set; }

        public void Construct()
        {
            _logger.Debug($"Module - CleanupSystem - Construct");

            FlowControl?.AssetUnloadingStarted
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    // RemovingStageUseEntities();
                    EntityManager.CreateEntity(_removeStageUseEventEntityArcehtype);
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _removeStageUseEventEntityArcehtype = EntityManager.CreateArchetype(
                typeof(RemoveStageUse),
                typeof(StageUse));
        }

        protected override void OnUpdate() {}

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
