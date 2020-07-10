namespace JoyBrick.Walkio.Game.Move.FlowField
{

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Pathfinding;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    
    using GameMove = JoyBrick.Walkio.Game.Move;

    //
    [GameCommon.DoneSettingAssetWait("Stage")]
    //
    [DisableAutoCreation]
    public class SetupFlowFieldSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SetupFlowFieldSystem));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _levelSettingEntityQuery;
        private EntityArchetype _tileEntityArchetype;

        //
        private bool _canSetup;
        private bool _doingSetup;

        //
        public GameCommon.IFlowControl FlowControl { get; set; }
        
        //
        public bool ProvideExternalAsset { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"SetupFlowFieldSystem - Construct");

            //
            FlowControl.SettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"SetupFlowFieldSystem - Construct - Receive SettingAsset");

                    _canSetup = true;
                    _doingSetup = true;

                    SettingAsset();
                })
                .AddTo(_compositeDisposable);             
        }

        private void SettingAsset()
        {
            //
            Setup().ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(result =>
                {
                    _canSetup = false;
                    FlowControl.FinishSetting(new GameCommon.FlowControlContext
                    {
                        Name = "Stage"
                    });
                })
                .AddTo(_compositeDisposable);
        }

        private async Task Setup()
        {
            _logger.Debug($"SetupFlowFieldSystem - Setup");

            // var entity = _levelSettingEntityQuery.GetSingletonEntity();
            // var levelSetting = EntityManager.GetComponentData<Common.LevelSetting>(entity);
            //
            // var horizontalTileCount = 4;
            // var verticalTileCount = 4;
            //
            // var totalTileCount = horizontalTileCount * verticalTileCount;
            // // Setup flow field tile buffer
            // var tileBuffer = EntityManager.AddBuffer<GameCommon.FlowFieldTileBuffer>(entity);
            // tileBuffer.ResizeUninitialized(totalTileCount);

            while (_doingSetup)
            {
                await Task.Delay(System.TimeSpan.FromMilliseconds(100));
            }
        }

        protected override void OnCreate()
        {
            _logger.Debug($"SetupFlowFieldSystem - OnCreate");

            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            _tileEntityArchetype = EntityManager.CreateArchetype(
                typeof(FlowFieldTile),
                typeof(GameCommon.StageUse));
            
            _levelSettingEntityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<GameCommon.LevelSetting>() 
                }
            });
            
            
            RequireForUpdate(_levelSettingEntityQuery);
        }

        protected override void OnUpdate()
        {
            if (!_canSetup) return;

            if (!_doingSetup) return;
            
            var levelSettingEntity = _levelSettingEntityQuery.GetSingletonEntity();
            // var buffer = EntityManager.GetBuffer<GameCommon.FlowFieldTileBuffer>(levelSettingEntity);
            
            //
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();
            
            //
            Entities
                .WithAll<GameCommon.LevelSetting>()
                .ForEach((Entity entity) =>
                {
                    _logger.Debug($"SetupFlowFieldSystem - OnUpdate - Should setup flow field tile buffer for level setting");

                    var horizontalTileCount = 4;
                    var verticalTileCount = 4;

                    var totalTileCount = horizontalTileCount * verticalTileCount;
                    var tileBuffer = commandBuffer.AddBuffer<GameCommon.FlowFieldTileBuffer>(entity);
                    tileBuffer.ResizeUninitialized(totalTileCount);

                    for (var tv = 0; tv < verticalTileCount; ++tv)
                    {
                        for (var th = 0; th < horizontalTileCount; ++th)
                        {
                            var tileIndex = tv * horizontalTileCount + th;
                    
                            var tileEntity = commandBuffer.CreateEntity(_tileEntityArchetype);
                    
#if UNITY_EDITOR
                            World.DefaultGameObjectInjectionWorld.EntityManager.SetName(tileEntity, $"Tile Entity - Base");
#endif                    
                            tileBuffer[tileIndex] = tileEntity;
                        }
                    }
                    
                    _doingSetup = false;
                })
                // .Schedule();
                .WithoutBurst()
                .Run();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
