namespace JoyBrick.Walkio.Game.App.HudService.Main
{
    using System.Collections.Generic;
    using System.Linq;

    using UnityEngine;

    using Doozy.Engine;
    using UniRx;

    using Common = App.Common.Main;

    public class ModuleOperator :
        Zenject.IInitializable,
        System.IDisposable
    {
        //
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(ModuleOperator));

        //
        private readonly Zenject.DiContainer _diContainer;
        private readonly Zenject.SignalBus _signalBus;

        //
        private readonly ModulewideInstaller.Settings _settings;
        private readonly ModulewideSOI.Settings _soiSettings;

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

       //
       private readonly List<Doozy.Engine.Nody.GraphController> _graphControllerCollection = new List<Doozy.Engine.Nody.GraphController>();
       private readonly List<Doozy.Engine.UI.UICanvas> _uiCanvasCollection = new List<Doozy.Engine.UI.UICanvas>();


        public ModuleOperator(
            Zenject.DiContainer diContainer,
            Zenject.SignalBus signalBus,
            ModulewideInstaller.Settings settings,
            ModulewideSOI.Settings soiSettings)
        {
            _diContainer = diContainer;
            _signalBus = signalBus;
            _settings = settings;
            _soiSettings = soiSettings;
        }

        public void Initialize()
        {
            _logger.Debug($"App - [MO] - HudService - Initialize");

            //
            Message.AddListener<GameEventMessage>(OnDoozyMessage);

            //
            _signalBus
                .GetStream<Common.AllModuleSetupDoneSignal>()
                .Subscribe(x =>
                {
                    //
                    _logger.Debug($"App - [MO] - HudService - Receive AllModuleSetupDoneSignal");
                    HandleAllModuleSetupDoneSignal();
                })
                .AddTo(_compositeDisposable);

           //
           var graphControllerInstances = CreateInstances(_diContainer, _settings.parent, _soiSettings.graphControllerPrefabs);
           var uiCanvasInstances = CreateInstances(_diContainer, _settings.parent, _soiSettings.uiCanvasPrefabs);
           var graphControllerCollection =
               graphControllerInstances
                   .Select(x => x.GetComponent<Doozy.Engine.Nody.GraphController>())
                   .Where(x => x != null);
           var uiCanvasCollection =
               uiCanvasInstances
                   .Select(x =>
                   {
                       var canvas = x.GetComponent<Canvas>();
                       if (canvas != null)
                       {
                           if (canvas.renderMode == RenderMode.WorldSpace ||
                               canvas.renderMode == RenderMode.ScreenSpaceCamera)
                           {
                               canvas.worldCamera = _settings.camera;
                           }
                       }

                       var uiCanvas = x.GetComponent<Doozy.Engine.UI.UICanvas>();
                       return uiCanvas;
                   })
                   .Where(x => x != null);

            //
           _graphControllerCollection.AddRange(graphControllerCollection);
           _uiCanvasCollection.AddRange(uiCanvasCollection);

            //
            _signalBus.Fire(new Common.ModuleSetupDoneSignal
            {
                Id = "Hud Service"
            });
        }

        private void OnDoozyMessage(GameEventMessage message)
        {
            _logger.Debug($"App - [MO] - HudService - OnDoozyMessage");

            if (message != null && !string.IsNullOrEmpty(message.EventName))
            {
            }
        }

        private void HandleAllModuleSetupDoneSignal()
        {
        }

        public void Dispose()
        {
            //
            Message.RemoveListener<GameEventMessage>(OnDoozyMessage);

            _compositeDisposable?.Dispose();
        }

        //
       public static IEnumerable<GameObject> CreateInstances(
           Zenject.DiContainer diContainer,
           Transform parent,
           IEnumerable<GameObject> prefabs)
       {
          var instances =
               prefabs.Select(prefab =>
               {
                   var instance = diContainer.InstantiatePrefab(prefab);
                   instance.transform.SetParent(parent);

                   return instance;
               });

          return instances;
       }
    }
}
