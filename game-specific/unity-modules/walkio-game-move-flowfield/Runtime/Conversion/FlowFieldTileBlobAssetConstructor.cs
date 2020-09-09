namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    //
#if WALKIO_FLOWCONTROL_SYSTEM
    using GameCommon = JoyBrick.Walkio.Game.Common;
#endif

    public class FlowFieldTileBlobAssetConstructor : GameObjectConversionSystem
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(FlowFieldTileBlobAssetConstructor));

        private static void ConvertFromAuthoringData(
            Common.FlowFieldWorldBlobAssetAuthoring authoring,
            BlobBuilder blobBuilder,
            ref FlowFieldTileBlobAsset flowFieldTileBlobAsset)
        {
            var context = authoring.context;
        }
        
        private void AddToEntity(BlobAssetReference<FlowFieldTileBlobAsset> flowFieldTileBlobAssetReference)
        {
            var flowFieldWorldPropertyQuery = DstEntityManager.CreateEntityQuery(
                typeof(FlowFieldWorld),
                typeof(FlowFieldWorldProperty));
            var flowFieldWorldEntity = flowFieldWorldPropertyQuery.GetSingletonEntity();
            var flowFieldWorldProperty = flowFieldWorldPropertyQuery.GetSingleton<FlowFieldWorldProperty>();

            _logger.Debug($"FlowFieldTileBlobAssetConstructor - AddToEntity");

            DstEntityManager.SetComponentData(flowFieldWorldEntity, new FlowFieldWorldProperty
            {
                //
                Id = flowFieldWorldProperty.Id,
                TileCount = flowFieldWorldProperty.TileCount,
                TileCellCount = flowFieldWorldProperty.TileCellCount,
                TileCellSize = flowFieldWorldProperty.TileCellSize,
                
                OriginPosition = flowFieldWorldProperty.OriginPosition,
                PositionOffset = flowFieldWorldProperty.PositionOffset,
                
                //
                FlowFieldTileBlobAssetRef = flowFieldTileBlobAssetReference
            });
        }

        protected override void OnUpdate()
        {
            var authorings =
                GetEntityQuery(typeof(Common.FlowFieldWorldBlobAssetAuthoring))
                    .ToComponentArray<Common.FlowFieldWorldBlobAssetAuthoring>();

            if (authorings.Length == 0) return;

            _logger.Debug($"FlowFieldTileBlobAssetConstructor - OnUpdate");
            
            BlobAssetReference<FlowFieldTileBlobAsset> flowFieldTileBlobAssetReference;

            using (var blobBuilder = new BlobBuilder(Allocator.Temp))
            {
                ref var flowFieldTileBlobAsset = ref blobBuilder.ConstructRoot<FlowFieldTileBlobAsset>();

                var authoring = authorings[0];

                ConvertFromAuthoringData(authoring, blobBuilder, ref flowFieldTileBlobAsset);

                flowFieldTileBlobAssetReference = blobBuilder.CreateBlobAssetReference<FlowFieldTileBlobAsset>(Allocator.Persistent);
                
                //
                AddToEntity(flowFieldTileBlobAssetReference);
            }            
        }
    }
}
