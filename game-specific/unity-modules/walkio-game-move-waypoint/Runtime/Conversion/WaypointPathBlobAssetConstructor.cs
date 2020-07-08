namespace JoyBrick.Walkio.Game.Move.Waypoint
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

    public class WaypointPathBlobAssetConstructor : GameObjectConversionSystem
    {
        protected override void OnUpdate()
        {
            var authorings =
                GetEntityQuery(typeof(WaypointPathBlobAssetAuthoring))
                    .ToComponentArray<WaypointPathBlobAssetAuthoring>();

            if (authorings.Length == 0) return;

            BlobAssetReference<WaypointPathBlobAsset> waypointPathBlobAssetReference;

            using (var blobBuilder = new BlobBuilder(Allocator.Temp))
            {
                ref var waypointPathBlobAsset = ref blobBuilder.ConstructRoot<WaypointPathBlobAsset>();

                var authoring = authorings[0];

                ConvertFromAuthoringData(authoring, blobBuilder, ref waypointPathBlobAsset);

                waypointPathBlobAssetReference = blobBuilder.CreateBlobAssetReference<WaypointPathBlobAsset>(Allocator.Persistent);
                
                //
                AddToEntity(waypointPathBlobAssetReference);
            }
        }

        private void AddToEntity(BlobAssetReference<WaypointPathBlobAsset> waypointPathBlobAssetReference)
        {
            var waypointPathLookupAttachmentQuery = DstEntityManager.CreateEntityQuery(typeof(GameCommon.WaypointPathLookupAttachment));
            var waypointPathLookupAttachmentEntity = waypointPathLookupAttachmentQuery.GetSingletonEntity();

            DstEntityManager.AddComponentData(waypointPathLookupAttachmentEntity, new WaypointPathLookup
            {
                WaypointPathBlobAssetRef = waypointPathBlobAssetReference
            });
        }

        private static void ConvertFromAuthoringData(
            WaypointPathBlobAssetAuthoring authoring,
            BlobBuilder blobBuilder,
            ref WaypointPathBlobAsset waypointPathBlobAsset)
        {
            //
            var pathCount = authoring.waypointPaths.Count;
            var waypointPathArray = blobBuilder.Allocate(ref waypointPathBlobAsset.WaypointPathIndexPairs, pathCount);

            //
            var waypointCount = authoring.waypointPaths
                .Aggregate(0, (acc, next) => acc + next.waypoints.Count);
            var waypointDataArray = blobBuilder.Allocate(ref waypointPathBlobAsset.Waypoints, waypointCount);

            //
            var index = 0;
            for (var wPathIndex = 0; wPathIndex < authoring.waypointPaths.Count; ++wPathIndex)
            {
                var wPath = authoring.waypointPaths[wPathIndex];
                var startIndex = wPathIndex * 2 + 0;
                var endIndex = wPathIndex * 2 + 1;
                var waypointPathIndexPair = new WaypointPathIndexPair
                {
                    StartIndex = index
                };

                foreach (var wp in wPath.waypoints)
                {
                    waypointDataArray[index] = (float3) wp.location;

                    ++index;
                }

                waypointPathIndexPair.EndIndex = index - 1;

                waypointPathArray[wPathIndex] = waypointPathIndexPair;
            }
        }
    }
}
