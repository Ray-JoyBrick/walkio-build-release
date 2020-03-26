namespace JoyBrick.Walkio.Game.App.LocalizationService.Main
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
            Container.DeclareSignal<Common.ChangeLanguageSignal>();
            
            Container.Bind<Settings>().FromInstance(settings).AsSingle();

            Container.BindInterfacesAndSelfTo<ModuleOperator>().AsSingle();
#endif
        }
    }
}
