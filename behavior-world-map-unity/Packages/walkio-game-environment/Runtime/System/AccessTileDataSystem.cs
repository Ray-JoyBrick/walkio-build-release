namespace JoyBrick.Walkio.Game.Environment
{
    using Unity.Entities;
    using UnityEngine;

    public class AccessTileDataSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            Entities
                .WithAll<Environment>()
                .ForEach((ref WorldMapTileLookup worldMapTileLookup) =>
                {
                    //
                    ref var tileDataBlobAsset = ref worldMapTileLookup.TileDataBlobAssetRef.Value;

                    var count = tileDataBlobAsset.TileDatas.Length;
                    for (var i = 0; i < count; ++i)
                    {
                        var td = tileDataBlobAsset.TileDatas[i];
                        
                        Debug.Log($"type: {td.Type}, cost: {td.Cost}");
                    }
                })
                // .Schedule();
                .WithoutBurst()
                .Run();
        }
    }
}