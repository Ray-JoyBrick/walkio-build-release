namespace JoyBrick.Walkio.Game.Battle
{
    using System.Collections.Generic;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.PlayerLoop;
    
    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using GameEnvironment = JoyBrick.Walkio.Game.Environment;
    
    [DisableAutoCreation]
    public class ManageUnitSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(ManageUnitSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

        //
        private bool _canUpdate;

        //
        private readonly List<GameObject> _units = new List<GameObject>();

        public GameCommon.IFlowControl FlowControl { get; set; }

        public List<GameObject> Units => _units;

        public void Construct()
        {
            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"ManageUnitSystem - Construct - Receive DoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
            
            FlowControl.CleaningAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _canUpdate = false;
                    _units.Clear();
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;
            
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
