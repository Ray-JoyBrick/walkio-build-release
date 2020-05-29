namespace JoyBrick.Walkio.Game.Environment
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

        protected override void OnUpdate()
        {
            var authorings =
                GetEntityQuery(typeof(GridMapBlobAssetAuthoring))
                    .ToComponentArray<GridMapBlobAssetAuthoring>();
            
            if (authorings.Length == 0) return;

            //
            _logger.Debug($"GridMapBlobAssetConstructor - Constructor - OnUpdate - Authoring found, proceed");
            
            BlobAssetReference<GridMapBlobAsset> gridMapBlobAssetReference;

            using (var blobBuilder = new BlobBuilder(Allocator.Temp))
            {
                var  authoring = authorings[0];

                ref var gridMapBlobAsset = ref blobBuilder.ConstructRoot<GridMapBlobAsset>();

                var count = authoring.gridCells.Count;
                var gridMapIndexArray = blobBuilder.Allocate(ref gridMapBlobAsset.Indices, count);

                for (var i = 0; i < count; ++ i)
                {
                    gridMapIndexArray[i] = authoring.gridCells[i];
                }

                gridMapBlobAssetReference = blobBuilder.CreateBlobAssetReference<GridMapBlobAsset>(Allocator.Persistent);
            }

            //
            var environmentQuery = DstEntityManager.CreateEntityQuery(typeof(TheEnvironment));
            var environmentEntity = environmentQuery.GetSingletonEntity();

            DstEntityManager.AddComponentData(environmentEntity, new LevelGridMapLookup
            {
                GridMapBlobAssetRef = gridMapBlobAssetReference
            });
        }
    }
}
