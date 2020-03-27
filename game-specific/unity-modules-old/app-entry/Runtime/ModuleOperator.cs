namespace JoyBrick.Walkio.Game.App.Entry.Main
{
    using System.IO;
    using System.Threading.Tasks;

    using UnityEngine;
    using UnityEngine.Networking;

    using UniRx;
    using UniRx.Async;
    using UniRx.Diagnostics;

    using Common = App.Common.Main;

    public class ModuleOperator :
        Zenject.IInitializable,
        System.IDisposable,

        Common.IAppStatusProvider
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(ModuleOperator));

        //
        private readonly Zenject.SignalBus _signalBus;

        //
        // private readonly Common.IPersistentService _persistentService;

        //
        private readonly ModulewideInstaller.Settings _settings;
        private readonly ModulewideSOI.Settings _soiSettings;

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private int _moduleSetupDoneCount;

        #region Related to IAppStatusProvider

        public ReactiveProperty<bool> AllModuleDoneSetup => new ReactiveProperty<bool>(false);
        public ReactiveProperty<string> BuildVersion => _buildVersion;
        private readonly ReactiveProperty<string> _buildVersion = new ReactiveProperty<string>("0.0.0.0");

        #endregion

        //
        public ModuleOperator(
            Zenject.SignalBus signalBus,
            // Common.IPersistentService persistentService,
            ModulewideInstaller.Settings settings,
            ModulewideSOI.Settings soiSettings)
        {
            _signalBus = signalBus;
            // _persistentService = persistentService;
            _settings = settings;
            _soiSettings = soiSettings;
        }

        public void Initialize()
        {
            //
            ObservableLogger.Listener.LogToUnityDebug().AddTo(_compositeDisposable);

            ObservableLogger.Listener
                .Where(x => x.LogType == LogType.Warning)
                .Subscribe(x =>
                {
                })
                .AddTo(_compositeDisposable);

            ObservableLogger.Listener
                .Where(x => x.LogType == LogType.Error)
                .Subscribe(x =>
                {
                })
                .AddTo(_compositeDisposable);

            //
            _logger.Debug($"App - [MO] - Entry - Initialize");

            //
            _signalBus
                .GetStream<Common.ModuleSetupDoneSignal>()
                .Do(x =>
                {
                    var existed = _soiSettings.loadingModules.Exists(lm => lm.CompareTo(x.Id) == 0);
                    if (existed)
                    {
                        ++_moduleSetupDoneCount;
                    }
                })
                .Where(x => _moduleSetupDoneCount == _soiSettings.loadingModules.Count)
                .Subscribe(x =>
                {
                    //
                    AllModuleDoneSetup.Value = true;
                    _signalBus.TryFire<Common.AllModuleSetupDoneSignal>();
                    HandleAllModuleSetupDoneSignal();
                })
                .AddTo(_compositeDisposable);
        }

        private void HandleAllModuleSetupDoneSignal()
        {
            LoadStartData().ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(x =>
                {
                    //
                })
                .AddTo(_compositeDisposable);
        }

        private async Task LoadStartData()
        {
            _logger.Debug($"App - [MO] - Entry - LoadStartData");

            //
            await FetchBuildVersion();

            //
            RestoreLanguage();
        }

        private void RestoreLanguage()
        {
            var systemLanguage = Application.systemLanguage;
            // var storedLanguage = _persistentService.Language;
            var storedLanguage = "";
            if (string.IsNullOrEmpty(storedLanguage))
            {
                _signalBus.TryFire(new Common.ChangeLanguageSignal
                {
                    UseSystemLanguage = true,
                    SystemLanguage = systemLanguage,
                    Language = string.Empty,
                    ToPersist = true
                });
            }
            else
            {
                _signalBus.TryFire(new Common.ChangeLanguageSignal
                {
                    UseSystemLanguage = false,
                    SystemLanguage = SystemLanguage.Unknown,
                    Language = storedLanguage,
                    ToPersist = false
                });
            }
        }

        private static string StreamingAssetPath
        {
            get
            {
                _logger.Debug($"App - [MO] - Entry - StreamingAssetPath");
                var path = "";
                if (Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    path = $"{Application.streamingAssetsPath}";
                }
                else if (Application.platform == RuntimePlatform.Android)
                {
                    path = $"jar:file://{Application.dataPath}!/assets";
                }
                else if (
                    Application.platform == RuntimePlatform.LinuxEditor ||
                    Application.platform == RuntimePlatform.LinuxPlayer ||
                    Application.platform == RuntimePlatform.WindowsEditor ||
                    Application.platform == RuntimePlatform.WindowsPlayer ||
                    Application.platform == RuntimePlatform.OSXEditor ||
                    Application.platform == RuntimePlatform.OSXPlayer)
                {
                    path = $"file://{Application.dataPath}/StreamingAssets";
                }

                return path;
            }
        }

        // From Stackoverflow
        // https://stackoverflow.com/questions/50400634/unity-streaming-assets-ios-not-working
        private async Task FetchBuildVersion()
        {
            _logger.Debug($"App - [MO] - Entry - FetchBuildVersion");

            var filePath = Path.Combine(StreamingAssetPath, "build.txt");
            if (filePath.Contains("://"))
            {
                using (var uwr = UnityWebRequest.Get(filePath))
                {
                    var uwrao = await uwr.SendWebRequest();
                    var text = uwrao.downloadHandler.text;
                    BuildVersion.Value = text;

                    _logger.Debug($"App - [AO] - Entry - FetchBuildVersion - build version: {text}");
                }
            }
            else
            {
                var text = System.IO.File.ReadAllText(filePath);
                BuildVersion.Value = text;

                _logger.Debug($"App - [AO] - Entry - FetchBuildVersion - build version: {text}");
            }
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}
