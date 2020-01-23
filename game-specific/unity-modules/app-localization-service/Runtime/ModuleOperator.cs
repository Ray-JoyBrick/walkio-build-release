namespace JoyBrick.Walkio.Game.App.LocalizationService.Main
{
    using System.Threading.Tasks;

    using UnityEngine;

    using UniRx;

    using Common = App.Common.Main;

    public class ModuleOperator :
        Zenject.IInitializable,
        System.IDisposable,

        Common.ILocalizationService
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(ModuleOperator));

        //
        private readonly Zenject.SignalBus _signalBus;

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        public ModuleOperator(
            Zenject.SignalBus signalBus)
        {
            _signalBus = signalBus;
        }

        public void Initialize()
        {
            _logger.Debug($"App - [MO] - LocalizationService - Initialize");

            //
            _signalBus
                .GetStream<Common.AllModuleSetupDoneSignal>()
                .Subscribe(x =>
                {
                    //
                    _logger.Debug($"App - [MO] - LocalizationService - Initialize - Receive AllModuleSetupDoneSignal");
                    HandleAllModuleSetupDoneSignal();
                })
                .AddTo(_compositeDisposable);

            //
            _signalBus.GetStream<Common.ChangeLanguageSignal>()
                .Subscribe(x =>
                {
                    HandleChangeLanguage(x.UseSystemLanguage, x.SystemLanguage, x.Language, x.ToPersist);
                })
                .AddTo(_compositeDisposable);

            //
            _signalBus.Fire(new Common.ModuleSetupDoneSignal
            {
                Id = "Localization Service"
            });
        }

        private static string SystemLanguageToString(SystemLanguage systemLanguage)
        {
            string language = "";
            if (systemLanguage == SystemLanguage.English)
            {
                language = "English";
            }
            else if (systemLanguage == SystemLanguage.Japanese)
            {
                language = "Japanese";
            }
            else if (systemLanguage == SystemLanguage.ChineseSimplified)
            {
                language = "ChineseSimplified";
            }
            else if (systemLanguage == SystemLanguage.ChineseTraditional)
            {
                language = "ChineseTraditional";
            }

            return language;
        }

        private void HandleChangeLanguage(bool useSystemLanguage, SystemLanguage systemLanguage, string language, bool toPersist)
        {
            var adjustedLanguage = language;
            if (useSystemLanguage)
            {
                adjustedLanguage = SystemLanguageToString(systemLanguage);
            }

            if (toPersist)
            {
                _signalBus.TryFire(new Common.PersistLanguageSignal
                {
                    Language = adjustedLanguage
                });
            }

            ChangeLanguage(language);
        }

        private void HandleAllModuleSetupDoneSignal()
        {
            LoadStartData().ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(x =>
                {
//                    I2.Loc.LocalizationManager.Sources.Add();
                })
                .AddTo(_compositeDisposable);
        }

        private async Task LoadStartData()
        {
            _logger.Debug($"App - [MO] - LocalizationService - LoadStartData");
        }

        public void Dispose()
        {
            _compositeDisposable?.Dispose();
        }

        #region Related to ILocalizationService

        public void ChangeLanguage(string language)
        {
            I2.Loc.LocalizationManager.CurrentLanguage = language;

            //
            _signalBus.TryFire(new Common.LanguageIsAlertedSignal());
        }

        public string GetByKey(string key)
        {
            var result = key;

            if (string.IsNullOrEmpty(key))
            {
                result = string.Empty;

                return result;
            }

            var translatedContent = I2.Loc.LocalizationManager.GetTranslation(key);

            if (string.IsNullOrEmpty(translatedContent))
            {
                result = key;
            }
            else
            {
                result = translatedContent;
            }

            return result;
        }

        #endregion
    }
}
