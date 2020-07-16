namespace Game.Move.FlowField
{
    using Drawing;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;
    using UnityEngine;

    public struct FlowFieldTile : IComponentData
    {
        public int Index;
    }

    public struct FlowFieldTileCellBuffer : IBufferElementData
    {
        public int Value;
     
        public static implicit operator int(FlowFieldTileCellBuffer b) => b.Value;
        public static implicit operator FlowFieldTileCellBuffer(int v) => new FlowFieldTileCellBuffer { Value = v };        
    }

    [DisableAutoCreation]
    public class PresentWorldSystem : SystemBase
    {
        private BeginPresentationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();
            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginPresentationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer();
            var concurrentCommandBuffer = commandBuffer.ToConcurrent();
            
            using (var builder = DrawingManager.GetBuilder(true))
            {
                Entities
                    .WithAll<FlowFieldTile>()
                    .ForEach((Entity entity, Translation translation, DynamicBuffer<FlowFieldTileCellBuffer> buffer) =>
                    {
                        var cells = new int2(10, 10);
                        var totalSizes = new float2(10.0f, 10.0f);
                        builder.WireGrid(translation.Value, Quaternion.identity, cells, totalSizes);

                        var direction = new float3(0, 0, 1.0f);
                        var radius = 0.25f;
                        var color = new Color32(100, 255, 100, 255);

                        // var buffer = EntityManager.GetBuffer<FlowFieldTileCellBuffer>(entity);
                        // var buffer = GetBufferFromEntity<FlowFieldTileCellBuffer>();

                        for (var v = 0; v < 10; ++v)
                        {
                            for (var h = 0; h < 10; ++h)
                            {
                                var pos = new float3(
                                    translation.Value.x + h - 5.0f + 0.5f,
                                    0,
                                    translation.Value.z + v - 5.0f + 0.5f);

                                var posIndex = v * 10 + h;

                                var directionValue = buffer[posIndex];

                                if (directionValue == 0)
                                {
                                    direction = math.normalize(new float3(0, 0, 1.0f));
                                }
                                else if (directionValue == 1)
                                {
                                    direction = math.normalize(new float3(1.0f, 0, 1.0f));
                                }
                                else if (directionValue == 2)
                                {
                                    direction = math.normalize(new float3(1.0f, 0, 0.0f));
                                }
                                else if (directionValue == 3)
                                {
                                    direction = math.normalize(new float3(1.0f, 0, -1.0f));
                                }
                                else if (directionValue == 4)
                                {
                                    direction = math.normalize(new float3(0.0f, 0, -1.0f));
                                }
                                else if (directionValue == 5)
                                {
                                    direction = math.normalize(new float3(-1.0f, 0, -1.0f));
                                }
                                else if (directionValue == 6)
                                {
                                    direction = math.normalize(new float3(-1.0f, 0, 0.0f));
                                }
                                else if (directionValue == 7)
                                {
                                    direction = math.normalize(new float3(-1.0f, 0, 1.0f));
                                }


                                builder.Arrowhead(pos, direction, radius, color);

                                // builder.Label2D(pos, "Cool", Color.white);
                            }
                        }
                    })
                    .WithBurst()
                    .Run();
                    // .Schedule();
                    
                // .Run();
                // .WithoutBurst()
                // .Run();
            }
            //
            //
            // builder.Dispose();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
        }
    }
}

