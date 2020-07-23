namespace JoyBrick.Walkio.Game.FlowControl.Stage
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

    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;

    [GameFlowControl.DoneLoadingAssetWait("Stage")]
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
        public IFlowControl FlowControl { get; set; }

        public GameExtension.IExtensionService ExtensionService { get; set; }

        public string AtPart => "Stage";

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
                // Since the asset is provided, just notify instantly
                FlowControl?.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                {
                    Name = "Stage"
                });
            }
            else
            {
                // Load internally then notify
                InternalLoadAsset(
                    assetName,
                    () =>
                    {
                        FlowControl?.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                        {
                            Name = "Stage"
                        });
                    });
            }
        }

        //
        public void Construct()
        {
            _logger.Debug($"Module - LoadAssetSystem - Construct");

            FlowControl?.AssetLoadingStarted
                .Where(x => x.Name.Contains(AtPart))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - LoadAssetSystem - Construct - Receive AssetLoadingStarted");
                    var assetName = x.AssetName;
                    LoadingAsset(assetName);
                })
                .AddTo(_compositeDisposable);
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
