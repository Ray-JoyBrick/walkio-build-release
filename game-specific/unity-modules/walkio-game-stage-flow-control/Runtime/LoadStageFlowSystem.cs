namespace JoyBrick.Walkio.Game.StageFlowControl
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using UnityEngine.SceneManagement;

    using GameCommand = Walkio.Game.Command;
    using GameCommon = Walkio.Game.Common;
    
    [DisableAutoCreation]
    public class LoadStageFlowSystem : SystemBase
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(LoadStageFlowSystem));
        
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();
        
        //
        private GameObject _createNeutralPrefab;
        private GameObject _createTeamPrefab;
        private GameObject _playTimeCountdownPrefab;

        private GameObject _createNeutral;
        private GameObject _createTeam;
        private GameObject _playTimeCountdown;
        
        //
        public GameObject RefBootstrap { get; set; }
        public GameCommand.ICommandService CommandService { get; set; }
        public GameCommon.IFlowControl FlowControl { get; set; }

        //
        public void Construct()
        {
            _logger.Debug($"LoadStageFlowSystem - Construct");
            
            base.OnCreate();
            
            //
            FlowControl.LoadingAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    LoadingAsset();
                })
                .AddTo(_compositeDisposable);
            
            FlowControl.CleaningAsset
                .Where(x => x.Name.Contains("Stage"))
                .Subscribe(x =>
                {
                    RemovingAssets();
                })
                .AddTo(_compositeDisposable);

            CommandService.CommandStream
                .Where(x => (x as GameCommand.CreateNeutralForceUnit) != null)
                .Subscribe(x =>
                {
                    _logger.Debug($"LoadStageFlowSystem - Construct - Receive CreateNeutralForceUnit");
                })
                .AddTo(_compositeDisposable);
        }

        private void LoadingAsset()
        {
            //
            Load().ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(result =>
                {
                    //
                    (_createNeutralPrefab, _createTeamPrefab, _playTimeCountdownPrefab) = result;
                            
                    //
                    _createNeutral = GameObject.Instantiate(_createNeutralPrefab);
                    _createTeam = GameObject.Instantiate(_createTeamPrefab);
                    _playTimeCountdown = GameObject.Instantiate(_playTimeCountdownPrefab);
                    var scene = SceneManager.GetSceneByName("Entry");
                    if (scene.IsValid())
                    {
                        SceneManager.MoveGameObjectToScene(_createNeutral, scene);
                        SceneManager.MoveGameObjectToScene(_createTeam, scene);
                        SceneManager.MoveGameObjectToScene(_playTimeCountdown, scene);
                    }

                    SetReferenceToExtension(_createNeutral);
                    SetReferenceToExtension(_createTeam);
                    SetReferenceToExtension(_playTimeCountdown);
                    // AddCommandStreamAndInfoStream(_canvas);
                    //
                    // ExtractView();
                            
                    //
                    FlowControl.FinishLoadingAsset(new GameCommon.FlowControlContext
                    {
                        Name = "Stage"
                    });
                })
                .AddTo(_compositeDisposable);            
        }

        // private async Task<GameObject> Load()
        // {
        //     var addressName = "Game Flow";
        //     var handle = Addressables.LoadAssetAsync<GameObject>(addressName);
        //     var r = await handle.Task;
        
        //     return r;
        // }
        
        private async Task<T> GetAsset<T>(string addressName)
        {
            var handle = Addressables.LoadAssetAsync<T>(addressName);
            var r = await handle.Task;
        
            return r;
        }

        private async Task<(GameObject, GameObject, GameObject)> Load()
        {
            var createNeutralPrefabTask = GetAsset<GameObject>($"Create Neutral Force Unit");
            var createTeamPrefabTask = GetAsset<GameObject>($"Create Team Force Unit");
            var playTimeCountdownPrefabTask = GetAsset<GameObject>($"Play Time Countdown");
        
            var (createNeutralPrefab, createTeamPrefab, playTimeCountdownPrefab) =
                (await createNeutralPrefabTask, await createTeamPrefabTask, await playTimeCountdownPrefabTask);
        
            return (createNeutralPrefab, createTeamPrefab, playTimeCountdownPrefab);
        }
        
        // TODO: Move hard reference to PlayMakerFSM to somewhere else
        // TODO: Assign reference to FSM may need a better approach
        private void SetReferenceToExtension(GameObject inGO)
        {
            var pmfsms = new List<PlayMakerFSM>();

            // Canvas itself
            var comps = inGO.GetComponents<PlayMakerFSM>();
            if (comps.Length > 0)
            {
                pmfsms.AddRange(comps);
            }
            
            // Views under Canvas
            foreach (Transform child in inGO.transform)
            {
                comps = child.GetComponents<PlayMakerFSM>();
                if (comps.Length > 0)
                {
                    pmfsms.AddRange(comps);
                }
            }

            pmfsms.ForEach(x => SetFsmVariableValue(x, "zz_Command Service", RefBootstrap));
            pmfsms.ForEach(x => SetFsmVariableValue(x, "zz_Flow Service", RefBootstrap));
            pmfsms.Clear();
        }

        // TODO: Make this in some static class so that other class can access as well
        private static void SetFsmVariableValue(PlayMakerFSM pmfsm, string variableName, GameObject inValue)
        {
            var commandServiceVariables =
                pmfsm.FsmVariables.GameObjectVariables.Where(x => string.CompareOrdinal(x.Name, variableName) == 0);
                
            commandServiceVariables.ToList()
                .ForEach(x =>
                {
                    x.Value = inValue;
                });
        }

        protected override void OnUpdate() {}

        public void RemovingAssets()
        {
            //
            if (_createNeutralPrefab != null)
            {
                Addressables.ReleaseInstance(_createNeutralPrefab);
            }

            if (_createTeamPrefab != null)
            {
                Addressables.ReleaseInstance(_createTeamPrefab);
            }            
            
            if (_createTeamPrefab != null)
            {
                Addressables.ReleaseInstance(_playTimeCountdownPrefab);
            }

            //
            if (_createNeutral != null)
            {
                GameObject.Destroy(_createNeutral);
            }            
            if (_createTeam != null)
            {
                GameObject.Destroy(_createTeam);
            }            
            if (_playTimeCountdown != null)
            {
                GameObject.Destroy(_playTimeCountdown);
            }            
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            RemovingAssets();
            
            _compositeDisposable?.Dispose();
        }
    }
}
