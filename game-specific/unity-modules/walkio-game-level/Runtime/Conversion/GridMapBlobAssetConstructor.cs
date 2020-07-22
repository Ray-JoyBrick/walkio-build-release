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
            GridMapBlobAssetAuthoring authoring,
            BlobBuilder blobBuilder,
            ref GridMapBlobAsset gridMapBlobAsset)
        {
            _logger.Debug($"GridMapBlobAssetConstructor - ConvertFromAuthoringData");
            // var context = authoring.context;

            var cellCount = authoring.gridCells.Count;
            var gridMapContextArray = blobBuilder.Allocate(ref gridMapBlobAsset.GridMapContextArray, cellCount);

            for (var i = 0; i < authoring.gridCells.Count; ++i)
            {
                // gridMapContextArray[i] = new GridMapContext
                // {
                //     Index = authoring.gridCells[i]
                // };
                gridMapContextArray[i] = authoring.gridCells[i];
            }
        }

        private void AddToEntity(
            GridMapBlobAssetAuthoring authoring,
            BlobAssetReference<GridMapBlobAsset> gridMapBlobAssetReference)
        {
            var gridWorldPropertyQuery = DstEntityManager.CreateEntityQuery(typeof(GridWorldProperty));
            var gridWorldProperty = gridWorldPropertyQuery.GetSingletonEntity();

            // _logger.Debug($"GridMapBlobAssetConstructor - AddToEntity: gridWorldProperty: {gridWorldProperty}");
            //
            // _logger.Debug($"GridMapBlobAssetConstructor - AddToEntity: gridMapBlobAssetReference: {gridMapBlobAssetReference}");
            // _logger.Debug($"GridMapBlobAssetConstructor - AddToEntity: gridMapBlobAssetReference value: {gridMapBlobAssetReference.Value.ToString()}");
            //
            // var count = gridMapBlobAssetReference.Value.GridMapContextArray.Length;
            //
            // _logger.Debug($"GridMapBlobAssetConstructor - AddToEntity: count: {count}");

            DstEntityManager.SetComponentData(gridWorldProperty, new GridWorldProperty
            {
                CellCount = new int2(authoring.gridCellCount.x, authoring.gridCellCount.y),
                CellSize = (float2) authoring.gridCellSize,
                GridMapBlobAssetRef = gridMapBlobAssetReference
            });

            // Create an event entity to inform that the convert is done
            var eventEntityArchetype = DstEntityManager.CreateArchetype(
                typeof(GridMapBlobAssetConstructed));
            var eventEntity = DstEntityManager.CreateEntity(eventEntityArchetype);
        }

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
                ref var gridMapBlobAsset = ref blobBuilder.ConstructRoot<GridMapBlobAsset>();

                var authoring = authorings[0];

                ConvertFromAuthoringData(authoring, blobBuilder, ref gridMapBlobAsset);

                gridMapBlobAssetReference = blobBuilder.CreateBlobAssetReference<GridMapBlobAsset>(Allocator.Persistent);

                //
                AddToEntity(authoring, gridMapBlobAssetReference);
            }
        }
    }
}
