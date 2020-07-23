namespace JoyBrick.Walkio.Game.FlowControl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

#if WALKIO_EXTENSION
    using GameExtension = JoyBrick.Walkio.Game.Extension;
#endif

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

#if WALKIO_FLOWCONTROL
    [GameFlowControl.DoneLoadingAssetWait("App")]
#endif
    [DisableAutoCreation]
    public class LoadAssetSystem :
        SystemBase
        // GameCommon.ISystemContext
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(LoadAssetSystem));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        private ScriptableObject _assetData;

        //
#if WALKIO_FLOWCONTROL
        public IFlowControl FlowControl { get; set; }
#endif

        public GameExtension.IExtensionService ExtensionService { get; set; }

        public string AtPart => "App";

        //
        public bool ProvideExternalAsset { get; set; }

        private async Task<ScriptableObject> Load(string assetName)
        {
            //
            var assetDataTask = GameCommon.Utility.AssetLoadingHelper.GetAsset<ScriptableObject>(assetName);

            var assetData = await assetDataTask;

            return assetData;
        }

        private void InternalLoadAsset(
            string assetName,
            System.Action loadingDoneAction)
        {
            //
            Load(assetName).ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(result =>
                {
                    _assetData = result;


                    var usedFlowData = _assetData as Template.UsedFlowData;
                    usedFlowData.flowPrefabs.ForEach(prefab =>
                    {
                        var createdInstance = GameObject.Instantiate(prefab);
                        ExtensionService.SetReferenceToExtension(createdInstance);
                    });

                    loadingDoneAction();
                })
                .AddTo(_compositeDisposable);
        }

        private void LoadingAsset(string assetName)
        {
            if (ProvideExternalAsset)
            {
#if WALKIO_FLOWCONTROL
                // Since the asset is provided, just notify instantly
                FlowControl?.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                {
                    Name = "App"
                });
#endif
            }
            else
            {
                // Load internally then notify
                InternalLoadAsset(
                    assetName,
                    () =>
                    {
#if WALKIO_FLOWCONTROL
                        FlowControl?.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                        {
                            Name = "App"
                        });
#endif
                    });
            }
        }

        //
        public void Construct()
        {
            _logger.Debug($"Module - LoadAssetSystem - Construct");

#if WALKIO_FLOWCONTROL
            FlowControl?.AssetLoadingStarted
                .Where(x => x.Name.Contains(AtPart))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - LoadAssetSystem - Construct - Receive AssetLoadingStarted");
                    var assetName = x.AssetName;
                    LoadingAsset(assetName);
                })
                .AddTo(_compositeDisposable);
#endif
        }

        protected override void OnCreate()
        {
            base.OnCreate();
        }

        protected override void OnUpdate() { }

        protected override void OnDestroy()
        {
            base.OnDestroy();

            _compositeDisposable.Dispose();
        }
    }
}
