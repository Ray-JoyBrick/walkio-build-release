namespace JoyBrick.Walkio.Game.Environment
{
    using System.Collections.Generic;
    using System.Linq;
    using Unity.Entities;
    using UnityEngine;

    public class AccessTileDetailIndexSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<Environment>()
                .ForEach((ref WorldMapTileDetailIndexLookup worldMapTileDetailIndexLookup) =>
                {
                    //
                    ref var tileDataIndexBlobAsset = ref worldMapTileDetailIndexLookup.TileDetailIndexBlobAssetRef.Value;

                    var count = tileDataIndexBlobAsset.TileDetailIndices.Length;
                    var indices = new List<int>();
                    for (var i = 0; i < count; ++i)
                    {
                        var td = tileDataIndexBlobAsset.TileDetailIndices[i];
                        
                        indices.Add(td);
                    }

                    // var desc = indices.Aggregate("", (acc, next) => $"{acc}{next}");
                    // Debug.Log(desc);
                })
                // .Schedule();
                .WithoutBurst()
                .Run();
        }
    }
}