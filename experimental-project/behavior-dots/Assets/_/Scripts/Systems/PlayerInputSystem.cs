namespace JoyBrick.Walkio.Game.Main
{
    using System.Collections.Generic;

    using Unity.Burst;
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Jobs;
    using Unity.Mathematics;
    using Unity.Physics.Systems;
    using UnityEngine;

    //
    public struct DeviceInputEvent<T> where T : struct
    {
        public int DeviceId;
        public T InputValue;
    }

    public struct InputDeviceIdBufferElement : IBufferElementData
    {
        public int DeviceId;
    }
    
    //
    [System.Serializable]
    public struct GameplayInputs : IComponentData
    {
        public float2 Move;
        public float2 Look;
    
        public float Shoot;

        public bool ShootPressed;
        public bool ShootReleased;
    }
    
    //
    [System.Serializable]
    public struct PlayerTag : IComponentData
    {
    }

    //
    [DisableAutoCreation]
    [UpdateBefore(typeof(BuildPhysicsWorld))]
    // public class PlayerInputSystem : JobComponentSystem
    public class PlayerInputSystem : SystemBase
    {
        //
        public NativeList<DeviceInputEvent<float2>> MoveInputs;
        
        //
        public EntityQuery PlayersQuery;

        private float _cachedX;
        private float _cachedY;
        
        //
        public Entity CreatePlayer(List<int> deviceIds)
        {
            Debug.Log($"PlayerInputSystem - CreatePlayer");
            
            Entity playerEntity = EntityManager.CreateEntity();
            EntityManager.AddComponentData(playerEntity, new PlayerTag());
            EntityManager.AddComponentData(playerEntity, new GameplayInputs());

            DynamicBuffer<InputDeviceIdBufferElement> deviceIdsBuffer = EntityManager.AddBuffer<InputDeviceIdBufferElement>(playerEntity);
            foreach (var deviceId in deviceIds)
            {
                Debug.Log($"PlayerInputSystem - CreatePlayer - deviceId: {deviceId}");
                deviceIdsBuffer.Add(new InputDeviceIdBufferElement() { DeviceId = deviceId });
            }
        
            return playerEntity;
        }

        //
        protected override void OnCreate()
        {
            Debug.Log($"PlayerInputSystem - OnCreate");
            
            base.OnCreate();

            //
            MoveInputs = new NativeList<DeviceInputEvent<float2>>(Allocator.Persistent);
            
            //
            PlayersQuery = EntityManager.CreateEntityQuery(typeof(PlayerTag));

            //
            if(!Rewired.ReInput.isReady) return;
            
            Debug.Log($"PlayerInputSystem - OnCreate - ReInput is ready");
            Rewired.Player player = Rewired.ReInput.players.GetPlayer(0);
            
            player.AddInputEventDelegate(
                OnMoveReceivedX, Rewired.UpdateLoopType.Update, Rewired.InputActionEventType.AxisActive, "Horizontal");
            player.AddInputEventDelegate(
                OnMoveReceivedX, Rewired.UpdateLoopType.Update, Rewired.InputActionEventType.AxisInactive, "Horizontal");
            player.AddInputEventDelegate(
                OnMoveReceivedY, Rewired.UpdateLoopType.Update, Rewired.InputActionEventType.AxisActive, "Vertical");
            player.AddInputEventDelegate(
                OnMoveReceivedY, Rewired.UpdateLoopType.Update, Rewired.InputActionEventType.AxisInactive, "Vertical");
            
            player.AddInputEventDelegate(
                OnRotationReceivedX, Rewired.UpdateLoopType.Update, Rewired.InputActionEventType.AxisActive, "RotateHorizontal");
            player.AddInputEventDelegate(
                OnRotationReceivedX, Rewired.UpdateLoopType.Update, Rewired.InputActionEventType.AxisInactive, "RotateHorizontal");
            player.AddInputEventDelegate(
                OnRotationReceivedY, Rewired.UpdateLoopType.Update, Rewired.InputActionEventType.AxisActive, "RotateVertical");
            player.AddInputEventDelegate(
                OnRotationReceivedY, Rewired.UpdateLoopType.Update, Rewired.InputActionEventType.AxisInactive, "RotateVertical");
            
        }

        #region Rewired method callbacks

        private void OnMoveReceivedX(Rewired.InputActionEventData data)
        {
            _cachedX = data.GetAxis();
            OnMoveReceived(new Vector2(data.GetAxis(), 0f));
        }

        private void OnMoveReceivedY(Rewired.InputActionEventData data)
        {
            _cachedY = data.GetAxis();
            OnMoveReceived(new Vector2(0f, data.GetAxis()));
        }
        
        private void OnRotationReceivedX(Rewired.InputActionEventData data) =>
            OnRotationReceived(new Vector2(data.GetAxis(), 0f));

        private void OnRotationReceivedY(Rewired.InputActionEventData data) =>
            OnRotationReceived(new Vector2(0f, data.GetAxis()));

        #endregion
        
        #region Input related
        
        private void OnMoveReceived(Vector2 move)
        {
            // if (move == Vector2.zero) return;
            // // if (move != Vector2.zero)
            // // {
            // Debug.Log($"PlayerInputSystem - OnMoveReceived - move: {move}");
            // // }
            
            var e = new DeviceInputEvent<float2>
            {
                DeviceId = 0,
                // InputValue = (float2) move
                InputValue = new float2(_cachedX, _cachedY)
            };

            MoveInputs.Add(e);
        }
        
        private void OnRotationReceived(Vector2 rotate)
        {
            if (rotate != Vector2.zero)
            {
                Debug.Log($"PlayerInputSystem - OnRotationReceived - move: {rotate}");
            }
            
            // rotate *= rotateSpeed;
            // transform.Rotate(Vector3.up, -rotate.x, Space.World);
            // transform.Rotate(Vector3.right, rotate.y, Space.World);
        }
        
        #endregion

        protected override void OnDestroy()
        {
            base.OnDestroy();

            if (MoveInputs.IsCreated)
            {
                MoveInputs.Dispose();
            }

            //
            if(!Rewired.ReInput.isReady) return;

            Rewired.Player player = Rewired.ReInput.players.GetPlayer(0);

            player.RemoveInputEventDelegate(OnMoveReceivedX);
            player.RemoveInputEventDelegate(OnMoveReceivedY);
            player.RemoveInputEventDelegate(OnRotationReceivedX);
            player.RemoveInputEventDelegate(OnRotationReceivedY);
        }
        
        //
        protected override void OnStartRunning()
        {
            base.OnStartRunning();
            
            if(!Rewired.ReInput.isReady) return;
            
            Debug.Log($"PlayerInputSystem - OnStartRunning - ReInput is ready");
            Rewired.Player player = Rewired.ReInput.players.GetPlayer(0);

            // Only few maps should be manipulated here, but manipulate them all here
            player.controllers.maps.SetAllMapsEnabled(true);
        }

        protected override void OnStopRunning()
        {
            base.OnStopRunning();

            if(!Rewired.ReInput.isReady) return;
            
            Debug.Log($"PlayerInputSystem - OnStopRunning - ReInput is ready");
            Rewired.Player player = Rewired.ReInput.players.GetPlayer(0);

            // Only few maps should be manipulated here, but manipulate them all here
            player.controllers.maps.SetAllMapsEnabled(false);
        }

        //
        // [BurstCompile]
        struct PlayerInputJob : IJobForEach_BCC<InputDeviceIdBufferElement, PlayerTag, GameplayInputs>
        {
            [ReadOnly]
            [NativeDisableParallelForRestriction]
            public NativeList<DeviceInputEvent<float2>> MoveInputs;
            
            public void Execute(
                [ReadOnly] DynamicBuffer<InputDeviceIdBufferElement> inputDeviceIdBuffer,
                [ReadOnly] ref PlayerTag playerTag,
                ref GameplayInputs gameplayInputs)
            {
                foreach (InputDeviceIdBufferElement playerDeviceId in inputDeviceIdBuffer)
                {
                    foreach (DeviceInputEvent<float2> e in MoveInputs)
                    {
                        if (e.DeviceId == playerDeviceId.DeviceId)
                        {
                            // Debug.Log($"PlayerInputJob - deviceId: {e.DeviceId} inputValue: {e.InputValue}");
                            //
                            // var adjustX = (math.abs(e.InputValue.x) < 0.07f) ? 0 : e.InputValue.x;
                            // var adjustY = (math.abs(e.InputValue.y) < 0.07f) ? 0 : e.InputValue.y;
                            //
                            // gameplayInputs.Move.x = adjustX;
                            // gameplayInputs.Move.y = adjustY;
                            gameplayInputs.Move = e.InputValue;
                        }
                    }
                }
            }
        }

        //
        protected override void OnUpdate()
        {
            var job = new PlayerInputJob
            {
                MoveInputs = MoveInputs
            };
        
            Dependency = job.Schedule(this, Dependency);
            
            Dependency.Complete();
            
            MoveInputs.Clear();
        }
        
        // protected override JobHandle OnUpdate(JobHandle inputDeps)
        // {
        //     var job = new PlayerInputJob
        //     {
        //         MoveInputs = MoveInputs
        //     };
        //
        //     inputDeps = job.Schedule(this, inputDeps);
        //
        //     inputDeps.Complete();
        //
        //     MoveInputs.Clear();
        //
        //     return inputDeps;
        // }
    }
}
