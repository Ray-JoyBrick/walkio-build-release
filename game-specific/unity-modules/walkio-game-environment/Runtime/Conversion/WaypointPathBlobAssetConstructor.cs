namespace JoyBrick.Walkio.Game.Environment
{
    using System.Linq;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    // using GameTemplate = JoyBrick.Walkio.Game.Template;
    
    [UpdateInGroup(typeof(GameObjectAfterConversionGroup))]
    public class WaypointPathBlobAssetConstructor : GameObjectConversionSystem
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(WaypointPathBlobAssetConstructor));

        protected override void OnUpdate()
        {
            var authorings =
                GetEntityQuery(typeof(WaypointPathBlobAssetAuthoring))
                    .ToComponentArray<WaypointPathBlobAssetAuthoring>();

            if (authorings.Length == 0) return;

            //
            _logger.Debug($"WaypointPathBlobAsset - Constructor - OnUpdate - Authoring found, proceed");

            BlobAssetReference<WaypointPathBlobAsset> waypointPathBlobAssetReference;

            using (var blobBuilder = new BlobBuilder(Allocator.Temp))
            {
                ref var waypointPathBlobAsset = ref blobBuilder.ConstructRoot<WaypointPathBlobAsset>();
                
                // TileDataBlobAssetAuthoring itself is MonBehaviour + IConvertGameObjectToEntity and once being
                // converted into entity, that scriptable object which is ScriptableObject + IComponentData
                // becoming part of the entity.
                // The question is, can this conversion takes place only when ScriptableObject is loaded?
                // var  waypointPathAuthoring = GetEntityQuery(typeof(WaypointPathBlobAssetAuthoring)).ToComponentArray<WaypointPathBlobAssetAuthoring>()[0];
                var  authoring = authorings[0];

                var pathCount = authoring.waypointDataAsset.waypointPaths.Count;
                var pathEndStartCount = pathCount * 2;
                var waypointPathArray = blobBuilder.Allocate(ref waypointPathBlobAsset.WaypointPaths, pathCount);
                var waypointCount = authoring.waypointDataAsset.waypointPaths
                    .Aggregate(0, (acc, next) => acc + next.waypoints.Count);
                var waypointDataArray = blobBuilder.Allocate(ref waypointPathBlobAsset.Waypoints, waypointCount);

                var index = 0;

                for (var wPathIndex = 0; wPathIndex < authoring.waypointDataAsset.waypointPaths.Count; ++ wPathIndex)
                {
                    var wPath = authoring.waypointDataAsset.waypointPaths[wPathIndex];
                    var startIndex = wPathIndex * 2 + 0;
                    var endIndex = wPathIndex * 2 + 1;
                    var waypointPath = new WaypointPath();
                    
                    waypointPath.StartIndex = index;
                    
                    for (var i = 0; i < wPath.waypoints.Count; ++i)
                    {
                        var wp = wPath.waypoints[i];

                        waypointDataArray[index] = (float3) wp.location;

                        ++index;
                    }

                    waypointPath.EndIndex = index - 1;

                    waypointPathArray[wPathIndex] = waypointPath;
                }
                
                waypointPathBlobAssetReference = blobBuilder.CreateBlobAssetReference<WaypointPathBlobAsset>(Allocator.Persistent);
            }

            //
            var environmentQuery = DstEntityManager.CreateEntityQuery(typeof(TheEnvironment));
            var environmentEntity = environmentQuery.GetSingletonEntity();
            
            DstEntityManager.AddComponentData(environmentEntity, new LevelWaypointPathLookup
            {
                WaypointPathBlobAssetRef = waypointPathBlobAssetReference
            });
        }
    }
}
