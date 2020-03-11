namespace JoyBrick.Walkio.Game
{
    using System;
    using Unity.Entities;
    using Unity.Mathematics;

    [UpdateAfter(typeof(PlayerInputSystem))]
    public class DecideTargetSystem : SystemBase
    {
        private float2 _previousValidDirection = new float2(1.0f, 0);
        private float _previousStrength = 0;
        
        protected override void OnUpdate()
        {
            // var previousValidDirection = new float2(1.0f, 0);
            // var previousStrength = 0;

            Entities
                .WithAll<Player>()
                .ForEach((Entity entity, int entityInQueryIndex, ref PlayerInput playerInput, ref PlayerMovementRequest playerMovementRequest) =>
                {
                    var moveInput = playerInput.MoveInput;

                    var noMoveInput = (math.abs(moveInput.x) < 0.02f) && (math.abs(moveInput.y) < 0.02f);
                    
                    if (noMoveInput)
                    {
                        playerMovementRequest.Direction = _previousValidDirection;
                        playerMovementRequest.Strength = _previousStrength;
                    }
                    else
                    {
                        //
                        var direction = math.normalize(moveInput);
                        var strength = math.lengthsq(moveInput);

                        playerMovementRequest.Direction = direction;
                        playerMovementRequest.Strength = strength;

                        _previousValidDirection = direction;
                        _previousStrength = strength;
                    }
                })
                .WithoutBurst()
                .Run();
        }
    }
}
