namespace JoyBrick.Walkio.Game
{
    using System;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public struct MovementRequest : IComponentData
    {
        public float2 Direction;
        public float Strength;
    }

    [DisableAutoCreation]
    [AlwaysUpdateSystem]
    public class PlayerInputSystem : SystemBase
    {
        private Input.Generated.Controls _controls;
        private InputAction _moveAction;
        
        private float2 _previousMove;
        
        protected override void OnCreate()
        {
            base.OnCreate();
            
            _controls = new Input.Generated.Controls();
            _moveAction = _controls.DefaultMapping.Move;
        }

        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            
            _controls.Enable();
        }

        protected override void OnStopRunning()
        {
            base.OnStopRunning();
            
            _controls.Disable();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _controls.Disable();
        }

        protected override void OnUpdate()
        {
            var moveInput = _moveAction.triggered ? (float2)_moveAction.ReadValue<Vector2>() : float2.zero;

            // if (math.abs(moveInput.x) < 0.05f && math.abs(moveInput.y) < 0.05f)
            // {
            //     return;
            // }

            if (moveInput.x == _previousMove.x && moveInput.y == _previousMove.y) return;

            _previousMove = moveInput;            
            
            Entities
                .WithAll<Player>()
                .WithoutBurst()
                .ForEach((ref MovementRequest movementRequest) =>
                {
                    var direction = math.normalize(moveInput);
                    var strength = math.lengthsq(moveInput);
                    
                    Debug.Log($"direction: {direction} strength: {strength}");

                    movementRequest.Direction = direction;
                    movementRequest.Strength = strength;
                })
                // .Schedule();
                .Run();
        }
    }
}
