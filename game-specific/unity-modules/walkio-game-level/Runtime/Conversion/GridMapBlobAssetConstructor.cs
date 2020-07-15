namespace JoyBrick.Walkio.Game.Level
{
    using System.Linq;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    // using GameTemplate = JoyBrick.Walkio.Game.Template;
    
    [UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
    public class GridMapBlobAssetConstructor : GameObjectConversionSystem
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(GridMapBlobAssetConstructor));

        private static void ConvertFromAuthoringData(
            Common.GridMapBlobAssetAuthoring authoring,
            BlobBuilder blobBuilder,
            ref Common.GridMapBlobAsset gridMapBlobAsset)
        {
            // var context = authoring.context;
        }
        
        private void AddToEntity(BlobAssetReference<Common.GridMapBlobAsset> gridMapBlobAssetReference)
        {
            var gridWorldPropertyQuery = DstEntityManager.CreateEntityQuery(typeof(Common.GridWorldProperty));
            var gridWorldProperty = gridWorldPropertyQuery.GetSingletonEntity();

            DstEntityManager.SetComponentData(gridWorldProperty, new Common.GridWorldProperty
            {
                GridMapBlobAssetRef = gridMapBlobAssetReference
            });
        }
        
        protected override void OnUpdate()
        {
            var authorings =
                GetEntityQuery(typeof(Common.GridMapBlobAssetAuthoring))
                    .ToComponentArray<Common.GridMapBlobAssetAuthoring>();
            
            if (authorings.Length == 0) return;

            //
            _logger.Debug($"GridMapBlobAssetConstructor - Constructor - OnUpdate - Authoring found, proceed");
            
            BlobAssetReference<Common.GridMapBlobAsset> gridMapBlobAssetReference;

            using (var blobBuilder = new BlobBuilder(Allocator.Temp))
            {
                ref var gridMapBlobAsset = ref blobBuilder.ConstructRoot<Common.GridMapBlobAsset>();

                var authoring = authorings[0];

                ConvertFromAuthoringData(authoring, blobBuilder, ref gridMapBlobAsset);

                gridMapBlobAssetReference = blobBuilder.CreateBlobAssetReference<Common.GridMapBlobAsset>(Allocator.Persistent);
                
                //
                AddToEntity(gridMapBlobAssetReference);
            }
        }
    }
}
