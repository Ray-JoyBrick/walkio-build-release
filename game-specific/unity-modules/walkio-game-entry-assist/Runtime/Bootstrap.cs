namespace JoyBrick.Walkio.Game.Assist
{
    using UniRx;
    using UnityEngine;

    public partial class Bootstrap :
        MonoBehaviour
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(Bootstrap));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        void Awake()
        {
            HandleSceneLoad();
        }

        void Start()
        {
            _logger.Debug($"Bootstrap Assist - Start");

            _assistable?.SetupBeforeEcs
                .Subscribe(x =>
                {
                    //
                    SetupEcsWorldContext();
                    SetupEcsWorldSystem();

                    //
                    StartGameFlow();
                })
                .AddTo(_compositeDisposable);

            _assistable?.SetupAfterEcs
                .Subscribe(x =>
                {
                    //
                    HandleSetupAfterEcs();
                })
                .AddTo(_compositeDisposable);
        }

        private void OnDestroy()
        {
            CleanUpEcsWorldContext();

            _compositeDisposable?.Dispose();
        }
    }
}
