namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using System.Collections.Generic;
    using UniRx;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;
    using UnityEngine.PlayerLoop;
    
    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    // using GameEnvironment = JoyBrick.Walkio.Game.Environment;
    
    [DisableAutoCreation]
    public class AssignFlowFieldTileToTeamUnitSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(AssignFlowFieldTileToTeamUnitSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private BeginInitializationEntityCommandBufferSystem _entityCommandBufferSystem;

        //
        private bool _canUpdate;

        public GameCommon.IFlowControl FlowControl { get; set; }

        public void Construct()
        {
            _logger.Debug($"AssignFlowFieldTileToTeamUnitSystem - Construct");

            //
            FlowControl.AllDoneSettingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _logger.Debug($"AssignFlowFieldTileToTeamUnitSystem - Construct - Receive DoneSettingAsset");
                    _canUpdate = true;
                })
                .AddTo(_compositeDisposable);
            
            FlowControl.CleaningAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    _canUpdate = false;
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnCreate()
        {
            _logger.Debug($"AssignFlowFieldTileToTeamUnitSystem - OnCreate");

            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginInitializationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            if (!_canUpdate) return;
            
            //
            var teamUnitToPathSystem = World.GetExistingSystem<TeamUnitToPathSystem>();
            var teamCount = teamUnitToPathSystem.CachedEntities.Count;
            if (teamCount == 0) return;

            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();

            // NativeHashMap<int, NativeHashMap<int, Entity>> teamFlowFieldTileHashMap =
            //     new NativeHashMap<int, NativeHashMap<int, Entity>>(
            //         teamCount, Allocator.Temp);
            //
            // foreach (var pair in teamUnitToPathSystem.CachedEntities)
            // {
            //     var teamId = pair.Key;
            //     var cachedTable = pair.Value;
            //     
            //     teamFlowFieldTileHashMap[teamId] = new NativeHashMap<int, Entity>(cachedTable.Count, Allocator.Temp);
            //
            //     var innerHashMap = teamFlowFieldTileHashMap[teamId];
            //
            //     foreach (var tilePair in cachedTable)
            //     {
            //         innerHashMap[tilePair.Key] = tilePair.Value;
            //     }
            // }

            var hGridCount = 128;
            var vGridCount = 192;

            Entities
                .WithAll<MoveOnFlowFieldTile>()
                // .WithNone<GameEnvironment.TeamLeader>()
                .ForEach((Entity entity, FlowFieldGroup flowFieldGroup, Translation translation) =>
                {
                    var hasTeamIdInCache = teamUnitToPathSystem.CachedEntities.ContainsKey(flowFieldGroup.GroupId);

                    if (hasTeamIdInCache)
                    {
                        var tileTable = teamUnitToPathSystem.CachedEntities[flowFieldGroup.GroupId];

                        var tileIndex =
                            Utility.PathTileHelper.TileIndex1d(
                                hGridCount, vGridCount, 1.0f, 1.0f,
                                10, 10, 1.0f, 1.0f,
                                translation.Value.x, translation.Value.z);

                        var hasCachedTileEntity = tileTable.ContainsKey(tileIndex);
                        if (hasCachedTileEntity)
                        {
                            var tileEntity = tileTable[tileIndex];
                            
                            commandBuffer.SetComponent(entity, new MoveOnFlowFieldTileProperty
                            {
                                OnTile = tileEntity
                            });
                        }
                    }
                })
                // .Schedule();
                .WithoutBurst()
                .Run();

            // Entities
            //     .WithAll<Unit, TeamForce>()
            //     .WithNone<GameEnvironment.TeamLeader>()
            //     .ForEach((Entity entity, ref GameEnvironment.MoveOnFlowFieldTile moveOnFlowFieldTile) =>
            //     {
            //         
            //     })
            //     // .Schedule();
            //     .WithoutBurst()
            //     .Run();

            // teamFlowFieldTileHashMap.Dispose();

            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
