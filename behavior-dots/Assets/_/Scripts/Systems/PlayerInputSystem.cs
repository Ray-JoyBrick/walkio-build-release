namespace JoyBrick.Walkio.Game.Main
{
    using Unity.Collections;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    //
    public struct DeviceInputEvent<T> where T : struct
    {
        public int DeviceId;
        public T InputValue;
    }
    
    //
    [System.Serializable]
    public struct PlayerTag : IComponentData
    {
    }

    //
    [DisableAutoCreation]
    public class PlayerInputSystem : SystemBase
    {
        //
        public NativeList<DeviceInputEvent<float2>> MoveInputs;

        //
        protected override void OnCreate()
        {
            Debug.Log($"PlayerInputSystem - OnCreate");
            
            base.OnCreate();

            //
            MoveInputs = new NativeList<DeviceInputEvent<float2>>(Allocator.Persistent);

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

        private void OnMoveReceivedX(Rewired.InputActionEventData data) =>
            OnMoveReceived(new Vector2(data.GetAxis(), 0f));

        private void OnMoveReceivedY(Rewired.InputActionEventData data) =>
            OnMoveReceived(new Vector2(0f, data.GetAxis()));
        
        private void OnRotationReceivedX(Rewired.InputActionEventData data) =>
            OnRotationReceived(new Vector2(data.GetAxis(), 0f));

        private void OnRotationReceivedY(Rewired.InputActionEventData data) =>
            OnRotationReceived(new Vector2(0f, data.GetAxis()));

        #endregion
        
        #region Input related
        
        private void OnMoveReceived(Vector2 move)
        {
            if (move != Vector2.zero)
            {
                Debug.Log($"PlayerInputSystem - OnMoveReceived - move: {move}");
            }
            
            var e = new DeviceInputEvent<float2>
            {
                DeviceId = 0,
                InputValue = (float2) move
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
        }

        protected override void OnStopRunning()
        {
            base.OnStopRunning();
        }

        //
        struct PlayerInputJob : IJobForEach<PlayerTag>
        {
            [ReadOnly]
            [NativeDisableParallelForRestriction]
            public NativeList<DeviceInputEvent<float2>> MoveInputs;
            
            public void Execute(
                [ReadOnly] ref PlayerTag playerTag)
            {
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
    }
}
