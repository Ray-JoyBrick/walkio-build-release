namespace JoyBrick.Walkio.Game
{
    using System;
    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using Unity.Transforms;

    [DisableAutoCreation]
    [UpdateAfter(typeof(PlayerInputSystem))]
    public class DecideTargetSystem : SystemBase
    {
        // private float2 _previousValidDirection = new float2(1.0f, 0);
        // private float _previousStrength = 0;
        
        private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
        
        protected override void OnCreate()
        {
            base.OnCreate();

            _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
        
        protected override void OnUpdate()
        {
            var previousValidDirection = new float2(1.0f, 0);
            var previousStrength = 0;

            Entities
                .WithAll<Player>()
                .ForEach(
                    (Entity entity, int entityInQueryIndex,
                        ref PlayerInput playerInput, ref PlayerMovementRequest playerMovementRequest) =>
                    {
                        var moveInput = playerInput.MoveInput;
                        
                        var noMoveInput = (math.abs(moveInput.x) < 0.02f) && (math.abs(moveInput.y) < 0.02f);
                        
                        if (noMoveInput)
                        {
                            playerMovementRequest.Direction = previousValidDirection;
                            playerMovementRequest.Strength = previousStrength;
                        }
                        else
                        {
                            //
                            var direction = math.normalize(moveInput);
                            var strength = math.lengthsq(moveInput);
                        
                            playerMovementRequest.Direction = direction;
                            playerMovementRequest.Strength = strength;
                        
                            // _previousValidDirection = direction;
                            // _previousStrength = strength;
                        }
                    }
                )
                .Schedule();
            
            _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
        }
    }
}
