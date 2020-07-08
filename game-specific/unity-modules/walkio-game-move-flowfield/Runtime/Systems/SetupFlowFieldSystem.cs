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
        private EntityQuery _levelSettingEntityQuery;
        private EntityArchetype _tileEntityArchetype;

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

            var entity = _levelSettingEntityQuery.GetSingletonEntity();
            var levelSetting = EntityManager.GetComponentData<Common.LevelSetting>(entity);
            
            var horizontalTileCount = 4;
            var verticalTileCount = 4;

            var totalTileCount = horizontalTileCount * verticalTileCount;
            // Setup flow field tile buffer
            var tileBuffer = EntityManager.AddBuffer<GameCommon.FlowFieldTileBuffer>(entity);
            tileBuffer.ResizeUninitialized(totalTileCount);
            // for (var tv = 0; tv < verticalTileCount; ++tv)
            // {
            //     for (var th = 0; th < horizontalTileCount; ++th)
            //     {
            //         var tileIndex = tv * horizontalTileCount + th;
            //
            //         var tileEntity = EntityManager.CreateEntity(_tileEntityArchetype);
            //
            //         tileBuffer[tileIndex] = tileEntity;
            //     }
            // }
        }

        protected override void OnCreate()
        {
            _logger.Debug($"SetupFlowFieldSystem - OnCreate");

            base.OnCreate();
            
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
        }

        protected override void OnUpdate() {}

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable?.Dispose();
        }
    }
}
