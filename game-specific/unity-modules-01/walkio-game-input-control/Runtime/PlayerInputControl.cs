namespace JoyBrick.Walkio.Game.InputControl
{
    using System;
    using UniRx;
    using UnityEngine;
    using UnityEngine.InputSystem;

    public class PlayerInputControl :
        MonoBehaviour,
        Generated.Controls.INewactionmapActions
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(PlayerInputControl));

        private Generated.Controls _controls;
        private InputAction _moveAction;
        
        //
        public IObservable<Vector2> MoveDirectionChange => _notifyMoveDirectionChange.AsObservable();
        private readonly Subject<Vector2> _notifyMoveDirectionChange = new Subject<Vector2>();

        void Start()
        {
            _logger.Debug($"PlayerInputControl - Start");

            _controls = new Generated.Controls();
            _controls?.Newactionmap.SetCallbacks(this);
            
            _controls?.Enable();
        }

        // private void OnEnable()
        // {
        //     _logger.Debug($"PlayerInputControl - OnEnable");
        //     _controls?.Enable();
        // }

        private void OnDisable()
        {
            _logger.Debug($"PlayerInputControl - OnDisable");
            _controls?.Disable();
        }

        private void OnDestroy()
        {
            _controls?.Dispose();
        }

        //
        public void OnMove(InputAction.CallbackContext context)
        {
            var v = context.ReadValue<Vector2>();

            _logger.Debug($"PlayerInputControl - OnMove - v: {v}");

            _notifyMoveDirectionChange.OnNext(v);
        }
    }
}
