namespace JoyBrick.Walkio.Game.App.PersistentService.Main
{
    using System.Collections.Generic;

    using UnityEngine;

    using Zenject;

    [CreateAssetMenu(fileName = "ModulewideSOI", menuName = "Installers/App/Persistent/ModulewideSOI")]
    public class ModulewideSOI : Zenject.ScriptableObjectInstaller<ModulewideSOI>
    {
        [System.Serializable]
        public class Settings
        {
            public string dbName;
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
