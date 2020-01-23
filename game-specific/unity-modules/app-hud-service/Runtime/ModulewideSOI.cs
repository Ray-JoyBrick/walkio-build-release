namespace JoyBrick.Walkio.Game.App.HudService.Main
{
    using System.Collections.Generic;

    using UnityEngine;

    using Zenject;

    [CreateAssetMenu(fileName = "ModulewideSOI", menuName = "Installers/App/Hud/ModulewideSOI")]
    public class ModulewideSOI : Zenject.ScriptableObjectInstaller<ModulewideSOI>
    {
        [System.Serializable]
        public class Settings
        {
            public List<GameObject> graphControllerPrefabs;
            public List<GameObject> uiCanvasPrefabs;
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
