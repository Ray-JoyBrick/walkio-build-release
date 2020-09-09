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
    // using GameCommand = JoyBrick.Walkio.Game.Command;
    using GameCommon = JoyBrick.Walkio.Game.Common;

#if WALKIO_EXTENSION
    using GameExtension = JoyBrick.Walkio.Game.Extension;
#endif

    public partial class PrepareAssetSystem
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
                        createdInstance.name = createdInstance.name + $" - Stage";
#endif

                        // CommandService.AddCommandStreamProducer(createdInstance);
                        ExtensionService.SetReferenceToExtension(createdInstance);

                        SceneService.MoveToCurrentScene(createdInstance);

                        _flowInstances.Add(createdInstance);
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
                    Name = AtPart
                });
            }
            else
            {
                // Load internally then notify
                InternalLoadAsset(
                    assetName,
                    () =>
                    {
                        _logger.Debug($"Module - Flow Control - Stage - PrepareAssetSystem - LoadingAsset - send finish loading signal");
                        FlowControl?.FinishIndividualLoadingAsset(new FlowControlContext
                        {
                            Name = AtPart
                        });
                    });
            }
        }

        //
        private void RegisterToLoadFlow()
        {
            _logger.Debug($"Module - Flow Control - Stage - PrepareAssetSystem - RegisterToLoadFlow");

            FlowControl?.AssetLoadingStarted
                .Where(x => x.Name.Contains(AtPart))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Flow Control - Stage - PrepareAssetSystem - Receive AssetLoadingStarted");
                    // var assetName = x.AssetName;
                    var assetName = $"Flow Control - Stage/Used Flow Data";
                    LoadingAsset(assetName);
                })
                .AddTo(_compositeDisposable);
        }
    }
}
