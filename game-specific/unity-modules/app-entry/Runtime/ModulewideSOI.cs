namespace JoyBrick.Walkio.Game.App.Entry.Main
{
    using System.Collections.Generic;

    using UnityEngine;

    [CreateAssetMenu(fileName = "ModulewideSOI", menuName = "Installers/App/Entry/ModulewideSOI")]
    public class ModulewideSOI : Zenject.ScriptableObjectInstaller<ModulewideSOI>
    {
        [System.Serializable]
        public class PlatformVersion
        {
            // Use Odin Inspector to decorate these values for clarity
            // 0 is Android
            // 1 is iOS
            public List<string> versions;
        }

        [System.Serializable]
        public class Settings
        {
            public PlatformVersion platformVersion;
            public bool useAssistFlow;
            public List<string> loadingModules;
        }

        public Settings settings;

        public override void InstallBindings()
        {
#if USE_ZENJECT_BINDING

            Container.Bind<Settings>().FromInstance(settings).AsSingle();

#endif
        }
    }
}
