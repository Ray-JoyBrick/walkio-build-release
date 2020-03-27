namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Entities;
    using UnityEngine;

    [DisableAutoCreation]
    public class AccessTileDataSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<Environment>()
                .ForEach((ref WorldMapTileDetailLookup worldMapTileLookup) =>
                {
                    //
                    ref var tileDataBlobAsset = ref worldMapTileLookup.TileDetailBlobAssetRef.Value;

                    var count = tileDataBlobAsset.TileDetails.Length;
                    for (var i = 0; i < count; ++i)
                    {
                        var td = tileDataBlobAsset.TileDetails[i];
                        
                        Debug.Log($"type: {td.Type}, cost: {td.Cost}");
                    }
                })
                // .Schedule();
                .WithoutBurst()
                .Run();
        }
    }
}