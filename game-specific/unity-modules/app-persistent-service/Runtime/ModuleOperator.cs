namespace JoyBrick.Walkio.Game.App.PersistentService.Main
{
    using System.IO;
    using System.Threading.Tasks;

    using UnityEngine;

    // using LiteDB;
    using UniRx;

    using Common = App.Common.Main;

    public class ModuleOperator :
        Zenject.IInitializable,
        System.IDisposable,

        Common.IPersistentService
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(ModuleOperator));

        //
        private readonly Zenject.SignalBus _signalBus;

        //
        private readonly ModulewideInstaller.Settings _settings;
        private readonly ModulewideSOI.Settings _soiSettings;

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private string _connectionPath;

        #region Related to IPersistentService

        public string Language => "";

        #endregion

        //
        public ModuleOperator(
            Zenject.SignalBus signalBus,
            ModulewideInstaller.Settings settings,
            ModulewideSOI.Settings soiSettings)
        {
            _signalBus = signalBus;
            _settings = settings;
            _soiSettings = soiSettings;
        }

        public void Initialize()
        {
            _logger.Debug($"App - [MO] - PersistentService - Initialize");

            SetDbNameAsFallback($"game.db");

            //
            _signalBus
                .GetStream<Common.AllModuleSetupDoneSignal>()
                .Subscribe(x =>
                {
                    //
                    _logger.Debug($"App - [MO] - PersistentService - Initialize - Receive AllModuleSetupDoneSignal");
                    HandleAllModuleSetupDoneSignal();
                })
                .AddTo(_compositeDisposable);

            //
            _signalBus.Fire(new Common.ModuleSetupDoneSignal
            {
                Id = "Persistent Service"
            });
        }

        public void SetDbNameAsFallback(string dbName)
        {
            //
            var result = (_soiSettings.dbName == string.Empty) ? dbName : _soiSettings.dbName;
            _connectionPath = Path.Combine(Application.persistentDataPath, result);

            _logger.Debug($"App - [MO] - PersistentService - SetDbNameAsFallback - _connectionPath: {_connectionPath}");
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
            _logger.Debug($"App - [MO] - PersistentService - LoadStartData");
        }

        private void ResetData()
        {
            _logger.Debug($"App - [MO] - PersistentService - ResetData for connectionPath: {_connectionPath}");

            // using (var db = new LiteDatabase(_connectionPath))
            // {
            //     // db.DropCollection("storylines");
            // }
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }
    }
}
