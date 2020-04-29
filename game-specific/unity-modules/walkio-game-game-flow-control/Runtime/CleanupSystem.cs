namespace JoyBrick.Walkio.Game.GameFlowControl
{
    using System;
    using Command;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using GameCommon = JoyBrick.Walkio.Game.Common;

    [DisableAutoCreation]
    public class CleanupSystem : SystemBase
    {
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        private EntityArchetype _removeStageUseEventEntityArcehtype;

        // Should separate into
        // Flow state
        // Command
        public ICommandService CommandService { get; set; }
        
        public GameCommon.IFlowControl FlowControl { get; set; }

        public void Construct()
        {
            FlowControl.CleaningAsset
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
                typeof(GameCommon.RemoveStageUse),
                typeof(GameCommon.StageUse));
        }

        protected override void OnUpdate() {}

        // private void RemovingStageUseEntities()
        // {
        //     // var _stageUseEntityQuery = EntityManager.CreateEntityQuery(typeof(GameCommon.StageUse));
        // }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }
}
