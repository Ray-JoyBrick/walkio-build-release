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
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public class FlowFieldTileBlobAssetConstructor : GameObjectConversionSystem
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(FlowFieldTileBlobAssetConstructor));

        private static void ConvertFromAuthoringData(
            Common.FlowFieldWorldBlobAssetAuthoring authoring,
            BlobBuilder blobBuilder,
            ref Common.FlowFieldTileBlobAsset flowFieldTileBlobAsset)
        {
            var context = authoring.context;
        }
        
        private void AddToEntity(BlobAssetReference<Common.FlowFieldTileBlobAsset> flowFieldTileBlobAssetReference)
        {
            var flowFieldWorldPropertyQuery = DstEntityManager.CreateEntityQuery(typeof(Common.FlowFieldWorldProperty));
            var flowFieldWorldProperty = flowFieldWorldPropertyQuery.GetSingletonEntity();

            DstEntityManager.SetComponentData(flowFieldWorldProperty, new Common.FlowFieldWorldProperty
            {
                FlowFieldTileBlobAssetRef = flowFieldTileBlobAssetReference
            });
        }

        protected override void OnUpdate()
        {
            var authorings =
                GetEntityQuery(typeof(Common.FlowFieldWorldBlobAssetAuthoring))
                    .ToComponentArray<Common.FlowFieldWorldBlobAssetAuthoring>();

            if (authorings.Length == 0) return;
            
            BlobAssetReference<Common.FlowFieldTileBlobAsset> flowFieldTileBlobAssetReference;

            using (var blobBuilder = new BlobBuilder(Allocator.Temp))
            {
                ref var flowFieldTileBlobAsset = ref blobBuilder.ConstructRoot<Common.FlowFieldTileBlobAsset>();

                var authoring = authorings[0];

                ConvertFromAuthoringData(authoring, blobBuilder, ref flowFieldTileBlobAsset);

                flowFieldTileBlobAssetReference = blobBuilder.CreateBlobAssetReference<Common.FlowFieldTileBlobAsset>(Allocator.Persistent);
                
                //
                AddToEntity(flowFieldTileBlobAssetReference);
            }            
        }
    }
}
