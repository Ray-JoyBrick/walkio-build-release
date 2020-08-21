namespace JoyBrick.Walkio.Game.Assist
{
    using UniRx;
    using UnityEngine;
    using UnityEngine.SceneManagement;

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
            
#if !UNITY_EDITOR
            SceneManager.LoadSceneAsync(assistableSceneName, LoadSceneMode.Additive).AsObservable()
                .Delay(System.TimeSpan.FromMilliseconds(500))
                .Subscribe(asyncOp =>
                {
#endif
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
#if !UNITY_EDITOR
                })
                .AddTo(_compositeDisposable);
#endif

        }

        private void OnDestroy()
        {
            CleanUpEcsWorldContext();

            _compositeDisposable?.Dispose();
        }
    }
}
