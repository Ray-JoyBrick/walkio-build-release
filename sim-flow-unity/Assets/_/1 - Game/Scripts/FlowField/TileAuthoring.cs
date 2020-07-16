namespace Game.Move.FlowField
{
    using Unity.Entities;
    using UnityEngine;

    public class TileAuthoring :
        MonoBehaviour,
        IConvertGameObjectToEntity
    {
        public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
        {
            dstManager.AddComponentData(entity, new FlowFieldTile());
            var buffer = dstManager.AddBuffer<FlowFieldTileCellBuffer>(entity);
            
            buffer.ResizeUninitialized(10 * 10);
            
            for (var tv = 0; tv < 10; ++tv)
            {
                for (var th = 0; th < 10; ++th)
                {
                    var tileCellIndex = tv * 10 + th;

                    buffer[tileCellIndex] = Random.Range(0, 8);
                }
            }
        }
    }
}
