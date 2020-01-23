namespace JoyBrick.Walkio.Game.App.InputControlService.Main
{
    using UnityEngine;
    using UnityEngine.InputSystem;

    using UniRx;

    using Common = App.Common.Main;

    public class ModuleOperator :
        Zenject.IInitializable,
        System.IDisposable,

        Common.IPlayerControl
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(ModuleOperator));

        //
        private readonly Zenject.SignalBus _signalBus;

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private Generated.Controls _controls;

        //
        public System.IObservable<Vector2> PlayerMovePerformed => _notifyPlayerMovePerformed.AsObservable();
        private readonly Subject<Vector2> _notifyPlayerMovePerformed = new Subject<Vector2>();

        public System.IObservable<int> PlayerPauseHudPerformed => _notifyPlayerPauseHudPerformed.AsObservable();
        private readonly Subject<int> _notifyPlayerPauseHudPerformed = new Subject<int>();

        public System.IObservable<int> PlayerFirePerformed => _notifyPlayerFirePerformed.AsObservable();
        private readonly Subject<int> _notifyPlayerFirePerformed = new Subject<int>();

        public ModuleOperator(
            Zenject.SignalBus signalBus)
        {
            _signalBus = signalBus;
            _controls = new Generated.Controls();
        }

        public void Initialize()
        {
            _logger.Debug($"App - [MO] - InputControlService - Initialize");

            //
            _signalBus
                .GetStream<Common.AllModuleSetupDoneSignal>()
                .Subscribe(x =>
                {
                    //
                    _logger.Debug($"App - [MO] - InputControlService - Initialize - Receive AllModuleSetupDoneSignal");
                    HandleAllModuleSetupDoneSignal();
                })
                .AddTo(_compositeDisposable);

            //
            _controls.Enable();

            Observable
                .FromEvent<InputAction.CallbackContext>(
                    h => _controls.PlayerControl.Move.performed += h,
                    h => _controls.PlayerControl.Move.performed -= h)
                .Subscribe(x =>
                {
                    //
                    var v = x.ReadValue<Vector2>();
                    _notifyPlayerMovePerformed.OnNext(v);
                })
                .AddTo(_compositeDisposable);
//            Observable.EveryFixedUpdate()
//                .Subscribe(x =>
//                {
//                    var v = _controls.PlayerControl.Move.ReadValue<Vector2>();
//                    _notifyPlayerMovePerformed.OnNext(v);
//                })
//                .AddTo(_compositeDisposable);

            Observable
                .FromEvent<InputAction.CallbackContext>(
                    h => _controls.PlayerControl.PauseHud.performed += h,
                    h => _controls.PlayerControl.PauseHud.performed -= h)
                .Subscribe(x =>
                {
                    //
                    _notifyPlayerPauseHudPerformed.OnNext(1);
                })
                .AddTo(_compositeDisposable);

            Observable
                .FromEvent<InputAction.CallbackContext>(
                    h => _controls.PlayerControl.Fire.performed += h,
                    h => _controls.PlayerControl.Fire.performed -= h)
                .Subscribe(x =>
                {
                    //
                    _notifyPlayerFirePerformed.OnNext(1);
                })
                .AddTo(_compositeDisposable);

            //
            _signalBus.Fire(new Common.ModuleSetupDoneSignal
            {
                Id = "Input Control Service"
            });
        }

        private void HandleAllModuleSetupDoneSignal()
        {
        }

        public void Dispose()
        {
            //
            _controls.Disable();
            _controls.Dispose();

            _compositeDisposable?.Dispose();
        }
    }
}
