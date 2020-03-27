namespace JoyBrick.Walkio.Game.App.Entry.Main
{
    using UnityEngine;

    using Zenject;

    using Common = App.Common.Main;

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
            Zenject.SignalBusInstaller.Install(Container);

            Container.DeclareSignal<Common.AllModuleSetupDoneSignal>();
            Container.DeclareSignal<Common.ModuleSetupDoneSignal>();

            Container.Bind<Settings>().FromInstance(settings).AsSingle();

            Container.BindInterfacesAndSelfTo<ModuleOperator>().AsSingle();
#endif
        }
    }
}
