namespace JoyBrick.Walkio.Game.Stage.Entry.Main
{
    using System.Collections.Generic;

    using UnityEngine;

    [CreateAssetMenu(fileName = "ModulewideSOI", menuName = "Installers/Stage/Entry/ModulewideSOI")]
    public class ModulewideSOI : Zenject.ScriptableObjectInstaller<ModulewideSOI>
    {
        [System.Serializable]
        public class Settings
        {
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
