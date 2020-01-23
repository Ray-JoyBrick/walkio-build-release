namespace JoyBrick.Walkio.Game.App.Common.Main
{
    using UnityEngine;

    public class AllModuleSetupDoneSignal
    {
    }

    public class ModuleSetupDoneSignal
    {
        public string Id { get; set; }
    }

    //
    public class ChangeToSceneSignal
    {
        public string Title { get; set; }
    }

    //
    public class PersistLanguageSignal
    {
        public string Language { get; set; }
    }

    //
    public class ChangeLanguageSignal
    {
        public bool UseSystemLanguage { get; set; }
        public SystemLanguage SystemLanguage { get; set; }
        public string Language { get; set; }
        public bool ToPersist { get; set; }
    }

    public class LanguageIsAlertedSignal
    {
    }
}