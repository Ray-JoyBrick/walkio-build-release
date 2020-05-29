namespace JoyBrick.Walkio.Game.InputControl
{
    using System;
    using UniRx;
    using UnityEngine;
    using UnityEngine.Assertions;

    [RequireComponent(typeof(PlayerInputControl))]
    [RequireComponent(typeof(ControllingRoute))]
    public class FormRouteFromInput : MonoBehaviour
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(FormRouteFromInput));
        
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        private PlayerInputControl _playerInputControl;
        private ControllingRoute _controllingRoute;
        
        void Start()
        {
            _logger.Debug($"FormRouteFromInput - Start");

            _playerInputControl = GetComponent<PlayerInputControl>();
            _controllingRoute = GetComponent<ControllingRoute>();
            
            Assert.IsNotNull(_playerInputControl);
            Assert.IsNotNull(_controllingRoute);
            
            //
            _playerInputControl.MoveDirectionChange
                .Subscribe(x =>
                {
                    FormRoute(x);
                })
                .AddTo(_compositeDisposable);
        }

        private void FormRoute(Vector2 moveDirection)
        {
            if (moveDirection != Vector2.zero)
            {
                _logger.Debug($"FormRouteFromInput - FormRoute - moveDirection: {moveDirection}");

                var positionOffset2d = moveDirection.normalized * 100.0f;
                var positionOffset = new Vector3(positionOffset2d.x, 0, positionOffset2d.y);
                var newPosition = transform.position + positionOffset;
                
                _controllingRoute.AddPositionToPath(newPosition, true);
            }
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }
    }
}
