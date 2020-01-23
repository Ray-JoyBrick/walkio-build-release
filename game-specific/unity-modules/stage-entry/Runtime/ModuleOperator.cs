namespace JoyBrick.Walkio.Game.Stage.Entry.Main
{
    using UnityEngine;

    using UniRx;

    using AppCommon = App.Common.Main;
    using Common = Stage.Common.Main;

    public class ModuleOperator :
        Zenject.IInitializable,
        System.IDisposable
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(ModuleOperator));

        //
        private readonly Zenject.SignalBus _signalBus;

        //
        private readonly AppCommon.IAppStatusProvider _appStatusProvider;
        // private readonly AppCommon.ISceneStatusProvider _sceneStatusProvider;

        //
        private readonly ModulewideInstaller.Settings _settings;
        private readonly ModulewideSOI.Settings _soiSettings;

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private int _moduleSetupDoneCount = 0;

        //
        public ModuleOperator(
            Zenject.SignalBus signalBus,
            AppCommon.IAppStatusProvider appStatusProvider,
            // AppCommon.ISceneStatusProvider sceneStatusProvider,
            ModulewideInstaller.Settings settings,
            ModulewideSOI.Settings soiSettings)
        {
            _signalBus = signalBus;
            _appStatusProvider = appStatusProvider;
            // _sceneStatusProvider = sceneStatusProvider;
            _settings = settings;
            _soiSettings = soiSettings;
        }

        public void Initialize()
        {
            _logger.Debug($"Stage[M] - [MO] - Entry - Initialize");

            //
            var moduleSetupDoneStream =
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
                    .Select(x => 1L);


            var appModuleSetupDoneStream =
                Observable.TimerFrame(1)
                    .TakeUntil(_appStatusProvider.AllModuleDoneSetup.AsObservable().Where(y => y));

            var combinedModuleSetupDoneStream = moduleSetupDoneStream.Merge(appModuleSetupDoneStream);
            combinedModuleSetupDoneStream
                .Buffer(2)
                .Subscribe(_ =>
                {
                    _logger.Debug($"Stage[M] - [MO] - Entry - Initialize - App Modules all setup");
                    //
                    _signalBus.TryFire<Common.AllModuleSetupDoneSignal>();
                })
                .AddTo(_compositeDisposable);

            // //
            // Bolt.CustomEvent.Trigger(_sceneStatusProvider.RefGo, "At Stage Scene");
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}
