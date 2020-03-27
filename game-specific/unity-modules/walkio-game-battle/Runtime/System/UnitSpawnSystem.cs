namespace JoyBrick.Walkio.Game.Battle
{
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;

    [DisableAutoCreation]
    public class UnitSpawnSystem : SystemBase
    {
        private BeginSimulationEntityCommandBufferSystem _entityCommandBufferSystem;

        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<BeginSimulationEntityCommandBufferSystem>();
        }

        protected override void OnUpdate()
        {
            var commandBuffer = _entityCommandBufferSystem.CreateCommandBuffer().ToConcurrent();
            var deltaTime = Time.DeltaTime;

            Entities
                .ForEach((Entity entity, int entityInQueryIndex, Translation translation, ref UnitSpawner unitSpawner, ref UnitSpawnStyle unitSpawnStyle) =>
                {
                    var elapsedTime = unitSpawnStyle.CountDown + deltaTime;

                    unitSpawnStyle.CountDown = elapsedTime;

                    if (elapsedTime >= unitSpawnStyle.IntervalMax)
                    {
                        unitSpawnStyle.CountDown = 0;
                        
                        var unitEntity = commandBuffer.Instantiate(entityInQueryIndex, unitSpawner.Unit);

                        var unitPosition = new Translation
                        {
                            Value = translation.Value
                        };
                    
                        commandBuffer.SetComponent(entityInQueryIndex, unitEntity, unitPosition);
                    }
                    
                    commandBuffer.SetComponent(entityInQueryIndex, entity, unitSpawnStyle);

                })
                .Schedule();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);            
        }
    }
}