namespace JoyBrick.Walkio.Game
{
    using System;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.ResourceManagement.ResourceProviders;
    using UnityEngine.SceneManagement;

    [DisableAutoCreation]
    public class InitializeAppwideServiceSystem : SystemBase
    {
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        public ICommandService CommandService { get; set; }
        
        public void Construct()
        {
            base.OnCreate();

            CommandService.InitializingAppwideService
                .Subscribe(x =>
                {
                    //
                    CommandService.StartLoadingAppHud();
                })
                .AddTo(_compositeDisposable);

            var ob1 = CommandService.DoneLoadingAppHud;

            var combined = ob1;

            combined
                .Buffer(1)
                .Subscribe(x =>
                {
                    //
                    CommandService.StartSettingAppwideService();
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnUpdate()
        {
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }
    
    [DisableAutoCreation]
    public class LoadAppHudSystem : SystemBase
    {
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        public ICommandService CommandService { get; set; }

        //
        private GameObject _canvasPrefab;
        private GameObject _canvas;

        private View _loadView;

        //
        public void Construct()
        {
            base.OnCreate();

            //
            CommandService.LoadingAppHud
                .Subscribe(x =>
                {
                    //
                    Load().ToObservable()
                        .ObserveOnMainThread()
                        .SubscribeOnMainThread()
                        .Subscribe(result =>
                        {
                            //
                            _canvasPrefab = result;
                            
                            //
                            _canvas = GameObject.Instantiate(_canvasPrefab);
                            var commandStreamProducer = _canvas.GetComponent<ICommandStreamProducer>();
                            if (commandStreamProducer != null)
                            {
                                CommandService.AddCommandStreamProducer(commandStreamProducer);
                            }

                            var infoPresenter = _canvas.GetComponent<IInfoPresenter>();
                            if (infoPresenter != null)
                            {
                                CommandService.AddInfoStreamPresenter(infoPresenter);
                            }

                            foreach (Transform v in _canvas.transform)
                            {
                                var view = v.GetComponent<View>();
                                if (view != null)
                                {
                                    _loadView = view;
                                }
                            }
                            
                            //
                            CommandService.FinishLoadingAppHud();
                        })
                        .AddTo(_compositeDisposable);
                })
                .AddTo(_compositeDisposable);

            //
            CommandService.CommandStream
                .Where(x => (x as ActivateLoadingViewCommand) != null)
                .Subscribe(x =>
                {
                    var activateLoadingViewCommand = (x as ActivateLoadingViewCommand);
                    //
                    ActivateLoadingView(activateLoadingViewCommand.flag);
                })
                .AddTo(_compositeDisposable);
        }
        
        private async Task<GameObject> Load()
        {
            var addressName = $"Hud - App";
            var handle = Addressables.LoadAssetAsync<GameObject>(addressName);

            var r = await handle.Task;

            return r;
        }

        private void ActivateLoadingView(bool flag)
        {
            if (_loadView == null) return;

            if (flag)
            {
                PlayMakerFSM.BroadcastEvent("Activate_Loading_View");
            }
            else
            {
                PlayMakerFSM.BroadcastEvent("Deactivate_Loading_View");
            }
        }

        protected override void OnUpdate()
        {
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            Addressables.ReleaseInstance(_canvasPrefab);
            GameObject.Destroy(_canvas);
            
            _compositeDisposable?.Dispose();
        }
    }
    
    [DisableAutoCreation]
    public class SetupAppwideServiceSystem : SystemBase
    {
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        public ICommandService CommandService { get; set; }

        public void Construct()
        {
            base.OnCreate();

            CommandService.SettingAppwideService
                .Subscribe(x =>
                {
                    //
                    CommandService.FinishSetupAppwideService();
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnUpdate()
        {
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }
    
    [DisableAutoCreation]
    public class InitializePreparationwideServiceSystem : SystemBase
    {
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        public ICommandService CommandService { get; set; }
        
        public void Construct()
        {
            base.OnCreate();

            CommandService.InitializingPreparationwideService
                .Subscribe(x =>
                {
                    //
                    CommandService.StartLoadingPreparationHud();
                })
                .AddTo(_compositeDisposable);

            var ob1 = CommandService.DoneLoadingPreparationHud;

            var combined = ob1;

            combined
                .Buffer(1)
                .Subscribe(x =>
                {
                    //
                    CommandService.StartSettingPreparationService();
                })
                .AddTo(_compositeDisposable);
            
            CommandService.ActivateViewLoading(true);
        }

        protected override void OnUpdate()
        {
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }
    
    [DisableAutoCreation]
    public class LoadPreparationHudSystem : SystemBase
    {
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        public ICommandService CommandService { get; set; }

        //
        private GameObject _canvasPrefab;
        private GameObject _canvas;

        //
        public void Construct()
        {
            base.OnCreate();

            CommandService.LoadingPreparationHud
                .Subscribe(x =>
                {
                    //
                    Load().ToObservable()
                        .ObserveOnMainThread()
                        .SubscribeOnMainThread()
                        .Subscribe(result =>
                        {
                            //
                            _canvasPrefab = result;
                            
                            //
                            _canvas = GameObject.Instantiate(_canvasPrefab);
                            var commandStreamProducer = _canvas.GetComponent<ICommandStreamProducer>();
                            if (commandStreamProducer != null)
                            {
                                CommandService.AddCommandStreamProducer(commandStreamProducer);
                            }

                            var infoPresenter = _canvas.GetComponent<IInfoPresenter>();
                            if (infoPresenter != null)
                            {
                                CommandService.AddInfoStreamPresenter(infoPresenter);
                            }
                            
                            //
                            CommandService.FinishLoadingPreparationHud();
                        })
                        .AddTo(_compositeDisposable);
                })
                .AddTo(_compositeDisposable);
        }
        
        private async Task<GameObject> Load()
        {
            var addressName = $"Hud - Preparation";
            var handle = Addressables.LoadAssetAsync<GameObject>(addressName);

            var r = await handle.Task;

            return r;
        }

        protected override void OnUpdate()
        {
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            Addressables.ReleaseInstance(_canvasPrefab);
            GameObject.Destroy(_canvas);
            
            _compositeDisposable?.Dispose();
        }
    }
    
    [DisableAutoCreation]
    public class SetupPreparationwideServiceSystem : SystemBase
    {
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        public ICommandService CommandService { get; set; }

        public void Construct()
        {
            base.OnCreate();

            CommandService.SettingPreparationwideService
                .Subscribe(x =>
                {
                    //
                    CommandService.ActivateViewLoading(false);
                    
                    //
                    CommandService.FinishSetupPreparationwideService();
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnUpdate()
        {
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }
    
    [DisableAutoCreation]
    public class CleanupPreparationwideServiceSystem : SystemBase
    {
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        public ICommandService CommandService { get; set; }

        public void Construct()
        {
            base.OnCreate();

            CommandService.CleaningPreparationwideService
                .Subscribe(x =>
                {
                    // //
                    // CommandService.FinishSetupPreparationwideService();
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnUpdate()
        {
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }

    [DisableAutoCreation]
    public class InitializeStagewideServiceSystem : SystemBase
    {
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        public ICommandService CommandService { get; set; }
        
        public void Construct()
        {
            base.OnCreate();

            CommandService.InitializingStagewideService
                .Subscribe(x =>
                {
                    //
                    CommandService.StartLoadingStageHud();
                    CommandService.StartLoadingStageEnvironment();
                })
                .AddTo(_compositeDisposable);

            var ob1 = CommandService.DoneLoadingStageHud;
            var ob2 = CommandService.DoneLoadingStageEnvironment;

            var combined = ob1.Merge(ob2);

            combined
                .Buffer(2)
                .Subscribe(x =>
                {
                    //
                    CommandService.StartSettingStagewideService();
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnUpdate()
        {
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }
    
        [DisableAutoCreation]
    public class LoadStageHudSystem : SystemBase
    {
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        public ICommandService CommandService { get; set; }

        //
        private GameObject _canvasPrefab;
        private GameObject _canvas;

        //
        public void Construct()
        {
            base.OnCreate();

            CommandService.LoadingStageHud
                .Subscribe(x =>
                {
                    //
                    Load().ToObservable()
                        .ObserveOnMainThread()
                        .SubscribeOnMainThread()
                        .Subscribe(result =>
                        {
                            //
                            _canvasPrefab = result;
                            
                            //
                            _canvas = GameObject.Instantiate(_canvasPrefab);
                            var commandStreamProducer = _canvas.GetComponent<ICommandStreamProducer>();
                            if (commandStreamProducer != null)
                            {
                                CommandService.AddCommandStreamProducer(commandStreamProducer);
                            }

                            var infoPresenter = _canvas.GetComponent<IInfoPresenter>();
                            if (infoPresenter != null)
                            {
                                CommandService.AddInfoStreamPresenter(infoPresenter);
                            }
                            
                            //
                            CommandService.FinishLoadingStageHud();
                        })
                        .AddTo(_compositeDisposable);
                })
                .AddTo(_compositeDisposable);
        }
        
        private async Task<GameObject> Load()
        {
            var addressName = $"Hud - Stage";
            var handle = Addressables.LoadAssetAsync<GameObject>(addressName);

            var r = await handle.Task;

            return r;
        }

        protected override void OnUpdate()
        {
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            Addressables.ReleaseInstance(_canvasPrefab);
            GameObject.Destroy(_canvas);
            
            _compositeDisposable?.Dispose();
        }
    }
    
    [DisableAutoCreation]
    public class LoadStageEnvironmentSystem : SystemBase
    {
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        public ICommandService CommandService { get; set; }

        //
        // private GameObject _canvasPrefab;
        // private GameObject _canvas;

        //
        public void Construct()
        {
            base.OnCreate();

            CommandService.LoadingStageEnvironment
                .Subscribe(x =>
                {
                    //
                    Load().ToObservable()
                        .ObserveOnMainThread()
                        .SubscribeOnMainThread()
                        .Subscribe(result =>
                        {
                            // //
                            // _canvasPrefab = result;
                            //
                            // //
                            // _canvas = GameObject.Instantiate(_canvasPrefab);
                            // var commandStreamProducer = _canvas.GetComponent<ICommandStreamProducer>();
                            // if (commandStreamProducer != null)
                            // {
                            //     CommandService.AddCommandStreamProducer(commandStreamProducer);
                            // }
                            //
                            // var infoPresenter = _canvas.GetComponent<IInfoPresenter>();
                            // if (infoPresenter != null)
                            // {
                            //     CommandService.AddInfoStreamPresenter(infoPresenter);
                            // }
                            
                            //
                            CommandService.FinishLoadingStageEnvironment();
                        })
                        .AddTo(_compositeDisposable);
                })
                .AddTo(_compositeDisposable);
        }
        
        private async Task<GameObject> Load()
        {
            var addressName = $"Hud - Stage";
            var handle = Addressables.LoadAssetAsync<GameObject>(addressName);

            var r = await handle.Task;

            return r;
        }

        protected override void OnUpdate()
        {
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();

            // Addressables.ReleaseInstance(_canvasPrefab);
            // GameObject.Destroy(_canvas);
            
            _compositeDisposable?.Dispose();
        }
    }    
    
    [DisableAutoCreation]
    public class SetupStagewideServiceSystem : SystemBase
    {
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        public ICommandService CommandService { get; set; }

        public void Construct()
        {
            base.OnCreate();

            CommandService.SettingStagewideService
                .Subscribe(x =>
                {
                    //
                    CommandService.FinishSetupStagewideService();
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnUpdate()
        {
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }
    
    [DisableAutoCreation]
    public class CleanupStagewideServiceSystem : SystemBase
    {
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        public ICommandService CommandService { get; set; }

        public void Construct()
        {
            base.OnCreate();

            CommandService.CleaningStagewideService
                .Subscribe(x =>
                {
                    // //
                    // CommandService.FinishSetupPreparationwideService();
                })
                .AddTo(_compositeDisposable);
        }

        protected override void OnUpdate()
        {
        }
        
        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            _compositeDisposable?.Dispose();
        }
    }
    
    public partial class Bootstrap :
        ICommandService
    {
        public IObservable<int> InitializingAppwideService => _notifyInitializingAppwideService.AsObservable();
        private readonly Subject<int> _notifyInitializingAppwideService = new Subject<int>();
        public void StartInitializingAppwideService()
        {
            _notifyInitializingAppwideService.OnNext(1);
        }
        
        //
        public IObservable<int> LoadingAppHud => _notifyLoadingAppHud.AsObservable();
        private readonly Subject<int> _notifyLoadingAppHud = new Subject<int>();

        public void StartLoadingAppHud()
        {
            _notifyLoadingAppHud.OnNext(1);
        }

        public void FinishLoadingAppHud()
        {
            _notifyDoneLoadingAppHud.OnNext(1);
        }

        public IObservable<int> SettingAppwideService => _notifySettingAppwideService.AsObservable();
        private readonly Subject<int> _notifySettingAppwideService = new Subject<int>();

        public void StartSettingAppwideService()
        {
            _notifySettingAppwideService.OnNext(1);
        }

        public IObservable<int> DoneSettingAppwideService => _notifyDoneSettingAppwideService.AsObservable();
        private readonly Subject<int> _notifyDoneSettingAppwideService = new Subject<int>();

        public void FinishSetupAppwideService()
        {
            //
            _notifyDoneSettingAppwideService.OnNext(1);
            
            // Extension
            
            _notifyInitializingPreparationwideService.OnNext(1);
        }

        public void ActivateViewLoading(bool flag)
        {
            
        }


        public IObservable<int> InitializingPreparationwideService =>
            _notifyInitializingPreparationwideService.AsObservable();
        private readonly Subject<int> _notifyInitializingPreparationwideService = new Subject<int>();

        public IObservable<int> DoneLoadingAppHud => _notifyDoneLoadingAppHud.AsObservable();
        private readonly Subject<int> _notifyDoneLoadingAppHud = new Subject<int>();

        public IObservable<int> EnteringPreparationScene => _notifyEnteringPreparationScene.AsObservable();
        private readonly Subject<int> _notifyEnteringPreparationScene = new Subject<int>();
        public IObservable<int> ExitingPreparationScene => _notifyExitingPreparationScene.AsObservable();
        private readonly Subject<int> _notifyExitingPreparationScene = new Subject<int>();

        //
        public void StartEnteringPreparationScene()
        {
            _notifyEnteringPreparationScene.OnNext(1);
        }

        public void StartExitingPreparationScene()
        {
            _notifyExitingPreparationScene.OnNext(1);
        }

        public IObservable<int> LoadingPreparationHud => _notifyLoadingPreparationHud.AsObservable();
        private readonly Subject<int> _notifyLoadingPreparationHud = new Subject<int>();
        public IObservable<int> DoneLoadingPreparationHud => _notifyDoneLoadingPreparationHud.AsObservable();
        private readonly Subject<int> _notifyDoneLoadingPreparationHud = new Subject<int>();
        
        public void StartLoadingPreparationHud()
        {
            _notifyLoadingPreparationHud.OnNext(1);
        }

        public void FinishLoadingPreparationHud()
        {
            _notifyDoneLoadingPreparationHud.OnNext(1);
        }

        public IObservable<int> SettingPreparationwideService => _notifySettingPreparationwideService.AsObservable();
        private readonly Subject<int> _notifySettingPreparationwideService = new Subject<int>();

        public void StartSettingPreparationService()
        {
            _notifySettingPreparationwideService.OnNext(1);
        }

        public IObservable<int> DoneSettingPreparationwideService =>
            _notifyDoneSettingPreparationwideService.AsObservable();
        private readonly Subject<int> _notifyDoneSettingPreparationwideService = new Subject<int>();

        public void FinishSetupPreparationwideService()
        {
            _notifyDoneSettingPreparationwideService.OnNext(1);
        }

        public IObservable<int> CleaningPreparationwideService => _notifyCleaningPreparationwideService.AsObservable();
        private readonly Subject<int> _notifyCleaningPreparationwideService = new Subject<int>();

        //
        public IObservable<int> InitializingStagewideService => _notifyInitializingStagewideService.AsObservable();
        private readonly Subject<int> _notifyInitializingStagewideService = new Subject<int>();

        public void StartInitializingStagewideService()
        {
            _notifyInitializingStagewideService.OnNext(1);
        }

        public IObservable<int> LoadingStageHud => _notifyLoadingStageHud.AsObservable();
        private readonly Subject<int> _notifyLoadingStageHud = new Subject<int>();

        public IObservable<int> DoneLoadingStageHud => _notifyDoneLoadingStageHud.AsObservable();
        private readonly Subject<int> _notifyDoneLoadingStageHud = new Subject<int>();

        public void StartLoadingStageHud()
        {
            _notifyLoadingStageHud.OnNext(1);
        }

        public void FinishLoadingStageHud()
        {
            _notifyDoneLoadingStageHud.OnNext(1);
        }

        public IObservable<int> LoadingStageEnvironment => _notifyLoadingStageEnvironment.AsObservable();
        private readonly Subject<int> _notifyLoadingStageEnvironment = new Subject<int>();

        public IObservable<int> DoneLoadingStageEnvironment => _notifyDoneLoadingStageEnvironment.AsObservable();
        private readonly Subject<int> _notifyDoneLoadingStageEnvironment = new Subject<int>();

        public void StartLoadingStageEnvironment()
        {
            _notifyLoadingStageEnvironment.OnNext(1);
        }

        public void FinishLoadingStageEnvironment()
        {
            _notifyDoneLoadingStageEnvironment.OnNext(1);
        }

        public IObservable<int> SettingStagewideService => _notifySettingStagewideService.AsObservable();
        private readonly Subject<int> _notifySettingStagewideService = new Subject<int>();

        public void StartSettingStagewideService()
        {
            _notifySettingStagewideService.OnNext(1);
        }

        public IObservable<int> DoneSettingStagewideService =>
            _notifyDoneSettingStagewideService.AsObservable();
        private readonly Subject<int> _notifyDoneSettingStagewideService = new Subject<int>();

        public void FinishSetupStagewideService()
        {
            _notifyDoneSettingStagewideService.OnNext(1);
        }

        public IObservable<int> CleaningStagewideService => _notifyCleaningStagewideService.AsObservable();
        private readonly Subject<int> _notifyCleaningStagewideService = new Subject<int>();
    }
}
