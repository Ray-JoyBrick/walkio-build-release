namespace JoyBrick.Walkio.Game.Stage.Entry.Main
{
    using UnityEngine;

    using Zenject;

    using Common = Stage.Common.Main;

    public class ModulewideInstaller : Zenject.MonoInstaller<ModulewideInstaller>
    {
        [System.Serializable]
        public class Settings
        {
        }

        public Settings settings;

        public override void InstallBindings()
        {
#if USE_ZENJECT_BINDING
            Container.DeclareSignal<Common.AllModuleSetupDoneSignal>();
            Container.DeclareSignal<Common.ModuleSetupDoneSignal>();

            Container.Bind<Settings>().FromInstance(settings).AsSingle();

            Container.BindInterfacesAndSelfTo<ModuleOperator>().AsSingle();
#endif
        }
    }
}
