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
    using GameCommand = JoyBrick.Walkio.Game.Command;
    using GameCommon = JoyBrick.Walkio.Game.Common;

#if WALKIO_EXTENSION
    using GameExtension = JoyBrick.Walkio.Game.Extension;
#endif

    public partial class PrepareAssetSystem :
        SystemBase
        // GameCommon.ISystemContext
    {
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

#if UNITY_EDITOR
                        createdInstance.name = createdInstance.name.Replace("(Clone)", "");
                        createdInstance.name = createdInstance.name + $" - App";
#endif

                        // CommandService.AddCommandStreamProducer(createdInstance);
                        ExtensionService.SetReferenceToExtension(createdInstance);

                        SceneService.MoveToCurrentScene(createdInstance);
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
                FlowControl?.FinishIndividualLoadingAsset(new FlowControlContext
                {
                    Name = "App",
                    Description = "Flow Control"
                });
            }
            else
            {
                // Load internally then notify
                InternalLoadAsset(
                    assetName,
                    () =>
                    {
                        FlowControl?.FinishIndividualLoadingAsset(new FlowControlContext
                        {
                            Name = "App",
                            Description = "Flow Control"
                        });
                    });
            }
        }

        //
        private void RegisterToLoadFlow()
        {
            _logger.Debug($"Module - Flow Control - PrepareAssetSystem - RegisterToLoadFlow");

            FlowControl?.AssetLoadingStarted
                .Where(x => x.Name.Contains(AtPart))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Flow Control - PrepareAssetSystem - RegisterToLoadFlow - Receive AssetLoadingStarted");
                    // var assetName = x.AssetName;
                    var assetName = $"Flow Control - App/Used Flow Data";
                    LoadingAsset(assetName);
                })
                .AddTo(_compositeDisposable);
        }
    }
}
