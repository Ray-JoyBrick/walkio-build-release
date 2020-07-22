namespace JoyBrick.Walkio.Game.Move.FlowField
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
    
#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

#if WALKIO_LEVEL
    using GameLevel = JoyBrick.Walkio.Game.Level;
#endif
    // using GameMove = JoyBrick.Walkio.Game.Move;

    //
#if WALKIO_FLOWCONTROL
    [GameFlowControl.DoneSettingAssetWait("Stage")]
#endif    
    [DisableAutoCreation]
    public class SetupAssetSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(SetupAssetSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        private EntityQuery _gridWorldEntityQuery;
        // private EntityQuery _levelSettingEntityQuery;
        private EntityArchetype _tileEntityArchetype;

        //
        private bool _canSetup;
        private bool _doingSetup;

        //
#if WALKIO_FLOWCONTROL
        public GameFlowControl.IFlowControl FlowControl { get; set; }
#endif
        // public GameLevel.IGridWorldProvider GridWorldProvider { get; set; }
        public IFlowFieldWorldProvider FlowFieldWorldProvider { get; set; }

        private async Task Setup()
        {
            _logger.Debug($"Module - SetupAssetSystem - Setup");

            await Task.Delay(System.TimeSpan.FromMilliseconds(10000));

            var archetype = EntityManager.CreateArchetype(
                typeof(FlowFieldWorld),
                typeof(FlowFieldWorldProperty));

            var entity = EntityManager.CreateEntity(archetype);

#if UNITY_EDITOR
            EntityManager.SetName(entity, $"Flow Field World");
#endif

             var gridWorldProperty = _gridWorldEntityQuery.GetSingleton<GameLevel.GridWorldProperty>();

            // var gridWorldData = GridWorldProvider.GridWorldData as GameLevel.Template.GridWorldData;

            var gridCellCount = gridWorldProperty.CellCount;
            var gridCellSize = gridWorldProperty.CellSize;

            _logger.Debug($"Module - SetupAssetSystem - Setup - gridCellCount: {gridCellCount}");

            // for (var i = 0; i < gridWorldProperty.GridMapBlobAssetRef.Value.GridMapContextArray.Length; ++i)
            // {
            //     _logger.Debug($"Module - SetupAssetSystem - Setup - {gridWorldProperty.GridMapBlobAssetRef.Value.GridMapContextArray[i]}");
            // }

            var tileCellCount = int2.zero;
            var tileCellSize = float2.zero;
            var originalPosition = float3.zero;
            var positionOffset = float3.zero;
            // These are created based on the Grid World, so GridWorldProvider should be ready before these are assigned
            var flowFieldWorldData = FlowFieldWorldProvider.FlowFieldWorldData as Template.FlowFieldWorldData;
            if (flowFieldWorldData != null)
            {
                tileCellCount = new int2(flowFieldWorldData.tileCellCount.x, flowFieldWorldData.tileCellCount.y);
                tileCellSize = (float2) flowFieldWorldData.tileCellSize;
                originalPosition = (float3) flowFieldWorldData.originalPosition;
                positionOffset = (float3) flowFieldWorldData.positionOffset;
            }

            var tileCount =
                Utility.FlowFieldTileHelper.GetTileCountFromGrid2D(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize);
            // var tileCellCount = new int2(10, 10);
            // var tileCellSize = new float2(1.0f, 1.0f);
            // var originalPosition = new float3(0, 0, 0);
            // var positionOffset = new float3(-5, 0, -5);

            EntityManager.SetComponentData(entity, new FlowFieldWorldProperty
            {
                Id = 1,
                TileCount = tileCount,
                TileCellCount = tileCellCount,
                TileCellSize = tileCellSize,
                OriginPosition = originalPosition,
                PositionOffset = positionOffset
            });

            // var gridWorldProperty = EntityManager.GetComponentData<GameLevel.Common.GridWorldProperty>(GridWorldProvider.GridWorldEntity);
            //
            // var flowFieldTileBlobAssetAuthoring = FlowFieldWorldProvider.FlowFieldTileBlobAssetAuthoringPrefab
            //     .GetComponent<Common.FlowFieldWorldBlobAssetAuthoring>();
            //
            // if (flowFieldTileBlobAssetAuthoring != null)
            // {
            //     flowFieldTileBlobAssetAuthoring.context.gridTileCount =
            //         new Vector2Int(gridWorldProperty.GridTileCount.x, gridWorldProperty.GridTileCount.y);
            //     flowFieldTileBlobAssetAuthoring.context.gridTileCellCount =
            //         new Vector2Int(gridWorldProperty.GridTileCellCount.x, gridWorldProperty.GridTileCellCount.y);
            //
            //     // flowFieldTileBlobAssetAuthoring.context.gridCellDetals
            //
            //     GameObject.Instantiate(FlowFieldWorldProvider.FlowFieldTileBlobAssetAuthoringPrefab);
            // }
            //
            // while (_doingSetup)
            // {
            //     await Task.Delay(System.TimeSpan.FromMilliseconds(100));
            // }
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

#if WALKIO_FLOWCONTROL
                    FlowControl?.FinishIndividualSettingAsset(new GameFlowControl.FlowControlContext
                    {
                        Name = "Stage"
                    });
#endif
                })
                .AddTo(_compositeDisposable);
        }

        //
        public void Construct()
        {
            _logger.Debug($"Module - SetupAssetSystem - Construct");
#if WALKIO_FLOWCONTROL
            //
            FlowControl?.AssetSettingStarted
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"SetupAssetSystem - Construct - Receive SettingAsset");

                    _canSetup = true;
                    _doingSetup = true;

                    SettingAsset();
                })
                .AddTo(_compositeDisposable);
#endif
            SettingAsset();
        }

        protected override void OnCreate()
        {
            _logger.Debug($"Module - SetupAssetSystem - OnCreate");

            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();

            _tileEntityArchetype = EntityManager.CreateArchetype(
                typeof(FlowFieldTile),
                typeof(FlowFieldTileProperty));

            _gridWorldEntityQuery = GetEntityQuery(new EntityQueryDesc
            {
                All = new ComponentType[]
                {
                    ComponentType.ReadOnly<GameLevel.GridWorld>(),
                    ComponentType.ReadOnly<GameLevel.GridWorldProperty>()
                }
            });

            RequireForUpdate(_gridWorldEntityQuery);

            // _levelSettingEntityQuery = GetEntityQuery(new EntityQueryDesc
            // {
            //     All = new ComponentType[]
            //     {
            //         ComponentType.ReadOnly<GameCommon.LevelSetting>()
            //     }
            // });
            //
            // RequireForUpdate(_levelSettingEntityQuery);
        }

        protected override void OnUpdate()
        {
            if (!_canSetup) return;

            if (!_doingSetup) return;

            // var levelSettingEntity = _levelSettingEntityQuery.GetSingletonEntity();
            // var buffer = EntityManager.GetBuffer<GameCommon.FlowFieldTileBuffer>(levelSettingEntity);

            //
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

//             //
//             Entities
//                 .WithAll<GameCommon.LevelSetting>()
//                 .ForEach((Entity entity) =>
//                 {
//                     _logger.Debug($"SetupAssetSystem - OnUpdate - Should setup flow field tile buffer for level setting");
//
//                     var horizontalTileCount = 4;
//                     var verticalTileCount = 4;
//
//                     var totalTileCount = horizontalTileCount * verticalTileCount;
//                     var tileBuffer = commandBuffer.AddBuffer<GameCommon.FlowFieldTileBuffer>(entity);
//                     tileBuffer.ResizeUninitialized(totalTileCount);
//
//                     for (var tv = 0; tv < verticalTileCount; ++tv)
//                     {
//                         for (var th = 0; th < horizontalTileCount; ++th)
//                         {
//                             var tileIndex = tv * horizontalTileCount + th;
//
//                             var tileEntity = commandBuffer.CreateEntity(_tileEntityArchetype);
//
// // #if UNITY_EDITOR
// //                             World.DefaultGameObjectInjectionWorld.EntityManager.SetName(tileEntity, $"Tile Entity - Base");
// // #endif
//                             tileBuffer[tileIndex] = tileEntity;
//                         }
//                     }
//
//                     _doingSetup = false;
//                 })
//                 // .Schedule();
//                 .WithoutBurst()
//                 .Run();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
