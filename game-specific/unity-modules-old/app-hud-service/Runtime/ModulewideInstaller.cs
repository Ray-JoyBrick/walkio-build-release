namespace JoyBrick.Walkio.Game.App.HudService.Main
{
    using UnityEngine;

    using Zenject;

    public class ModulewideInstaller : Zenject.MonoInstaller<ModulewideInstaller>
    {
        [System.Serializable]
        public class Settings
        {
            public Transform parent;

            // Hud use camera
            public Camera camera;
            public GameObject operatorFacade;
        }

        public Settings settings;

        public override void InstallBindings()
        {
#if USE_ZENJECT_BINDING
            Container.Bind<Settings>().FromInstance(settings).AsSingle();

            Container.BindInterfacesAndSelfTo<ModuleOperator>().AsSingle();
#endif
        }
    }
}
