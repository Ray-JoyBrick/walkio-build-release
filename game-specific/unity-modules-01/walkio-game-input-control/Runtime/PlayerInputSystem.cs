// namespace JoyBrick.Walkio.Game.InputControl
// {
//     using System;
//     using Unity.Collections;
//     using Unity.Entities;
//     using Unity.Mathematics;
//     using UnityEngine;
//     using UnityEngine.InputSystem;
//     
//     public struct DeviceInputEvent<T> where T : struct
//     {
//         public int DeviceId;
//         public T InputValue;
//     }
//     
//     public struct PlayerMovementRequest : IComponentData
//     {
//         public float3 Origin;
//         
//         public float2 Direction;
//         public float Strength;
//     }
//
//     [DisableAutoCreation]
//     public class PlayerInputSystem :
//         SystemBase,
//         Generated.Controls.INewactionmapActions
//     {
//         // public Input.Generated.Controls InputActions;;
//         
//         private Generated.Controls _controls;
//         private InputAction _moveAction;
//         
//         private NativeList<DeviceInputEvent<float2>> _moveInputs;
//         
//         private float2 _previousMove;
//         
//         private EndSimulationEntityCommandBufferSystem _entityCommandBufferSystem;
//         
//         //
//         struct ControlInputsJob : IJobForEach_BC<InputDeviceIdBufferElement, PlayerInput>
//         {
//             [ReadOnly]
//             [NativeDisableParallelForRestriction]
//             public NativeList<DeviceInputEvent<float2>> MoveInputs;
//
//             public void Execute(
//                 [ReadOnly] DynamicBuffer<InputDeviceIdBufferElement> inputDeviceIdBufferElements, ref PlayerInput playerInput)
//             {
//                 foreach (var playerDeviceId in inputDeviceIdBufferElements)
//                 {
//                     foreach (var e in MoveInputs)
//                     {
//                         playerInput.MoveInput = e.InputValue;
//                         
//                         // Debug.Log($"input: {playerInput.MoveInput}");
//                     }
//                 }
//             }
//         }
//         
//         protected override void OnCreate()
//         {
//             base.OnCreate();
//           
//             _controls = new Generated.Controls();
//             // _moveAction = _controls.DefaultMapping.Move;
//             _controls.Newactionmap.SetCallbacks(this);
//
//             _moveInputs = new NativeList<DeviceInputEvent<float2>>(Allocator.Persistent);
//             
//             _entityCommandBufferSystem = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
//         }
//
//         protected override void OnStartRunning()
//         {
//             base.OnStartRunning();
//             
//             _controls.Enable();
//         }
//
//         protected override void OnStopRunning()
//         {
//             base.OnStopRunning();
//             
//             _controls.Disable();
//         }
//
//         protected override void OnDestroy()
//         {
//             base.OnDestroy();
//
//             if (_moveInputs.IsCreated)
//             {
//                 _moveInputs.Dispose();
//             }
//             
//             _controls.Dispose();
//         }
//         
//         protected override void OnUpdate()
//         {
//             // // Should not use triggered here
//             // var moveInput = _moveAction.triggered ? (float2)_moveAction.ReadValue<Vector2>() : float2.zero;
//             //
//             // // if (math.abs(moveInput.x) < 0.05f && math.abs(moveInput.y) < 0.05f)
//             // // {
//             // //     return;
//             // // }
//             //
//             // // if (math.abs(moveInput.x - _previousMove.x) < 0.02f && math.abs(moveInput.y - _previousMove.y) < 0.02f) return;
//             //
//             // _previousMove = moveInput;
//             //
//             // Entities
//             //     .WithAll<PlayerInput>()
//             //     .WithoutBurst()
//             //     .ForEach((Entity entity, int entityInQueryIndex, ref PlayerMovementRequest playerMovementRequest) =>
//             //     {
//             //         // var direction = (moveInput.x == 0 && moveInput.y == 0) ? float2.zero  : math.normalize(moveInput);
//             //         var direction = moveInput;
//             //         var strength = math.lengthsq(moveInput);
//             //         
//             //         Debug.Log($"direction: {direction} strength: {strength}");
//             //
//             //         playerMovementRequest.Direction = direction;
//             //         playerMovementRequest.Strength = strength;
//             //
//             //     })
//             //     // .Schedule();
//             //     .Run();
//             //
//             // _entityCommandBufferSystem.AddJobHandleForProducer(Dependency);
//
//             var job = new ControlInputsJob
//             {
//                 MoveInputs = _moveInputs
//             };
//
//             Dependency = job.Schedule(this, Dependency);
//             Dependency.Complete();
//             
//             _moveInputs.Clear();
//         }
//
//         //
//         public void OnMove(InputAction.CallbackContext context)
//         {
//             int deviceId = context.control.device.deviceId;
//
//             var v = context.ReadValue<Vector2>();
//
//             Debug.Log($"input: {v}");
//
//             DeviceInputEvent<float2> e = new DeviceInputEvent<float2>
//             {
//                 DeviceId = deviceId,
//                 InputValue = v
//             };
//
//             _moveInputs.Add(e);
//         }
//     }
// }
