namespace JoyBrick.Walkio.Game.Hud.Stage.Assist
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

#if WALKIO_FLOWCONTROL
    using GameFlowControl = JoyBrick.Walkio.Game.FlowControl;
#endif

    public partial class PrepareAssetSystem :
        SystemBase
        // GameCommon.ISystemContext
    {
        //
        public bool ProvideExternalAsset { get; set; }

        private async Task<ScriptableObject> Load(string hudAssetName)
        {
            //
            var hudSettingAssetName = hudAssetName;
            var hudSettingAssetTask = GameCommon.Utility.AssetLoadingHelper.GetAsset<ScriptableObject>(hudSettingAssetName);

            var hudSettingAsset = await hudSettingAssetTask;

            return hudSettingAsset;
        }

        private void InternalLoadAsset(
            string hudAssetName,
            System.Action loadingDoneAction)
        {
            //
            Load(hudAssetName).ToObservable()
                .ObserveOnMainThread()
                .SubscribeOnMainThread()
                .Subscribe(result =>
                {
                    _hudSettingDataAsset = result;

                    _hudData = (_hudSettingDataAsset as HudData);

                    if (_hudData != null)
                    {
                        _logger.Debug($"Module Assist - Hud - Stage - PrepareAssetSystem - InternalLoadAsset - hud data is not null");

                        _canvasPrefab = _hudData.canvasPrefab;
                        _canvas = GameObject.Instantiate(_canvasPrefab);

#if UNITY_EDITOR
                        _canvas.name = _canvas.name.Replace("(Clone)", "");
                        _canvas.name = _canvas.name + $" - Stage Assist";
#endif

                        CommandService.AddCommandStreamProducer(_canvas);
                        CommandService.AddInfoStreamPresenter(_canvas);
                        ExtensionService.SetReferenceToExtension(_canvas);

                        SceneService.MoveToCurrentScene(_canvas);

                        //
                        Utility.HudHelper.ExtractView(_canvas);
                    }
                    else
                    {
                        _logger.Debug($"Module Assist - Hud - Stage - PrepareAssetSystem - InternalLoadAsset - hud data is null");
                    }

                    loadingDoneAction();
                })
                .AddTo(_compositeDisposable);
        }

        private void LoadingAsset(string hudAssetName)
        {
            if (ProvideExternalAsset)
            {
                // Since the asset is provided, just notify instantly
#if WALKIO_FLOWCONTROL
                FlowControl.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                {
                    Name = "Stage"
                });
#endif
            }
            else
            {
                // Load internally then notify
                InternalLoadAsset(
                    hudAssetName,
                    () =>
                    {
#if WALKIO_FLOWCONTROL
                        FlowControl.FinishIndividualLoadingAsset(new GameFlowControl.FlowControlContext
                        {
                            Name = "Stage"
                        });
#endif
                    });
            }
        }

        //
        private void RegisterToLoadFlow()
        {
            _logger.Debug($"Module - Hud App - PrepareAssetSystem - RegisterToLoadFlow");

#if WALKIO_FLOWCONTROL
            FlowControl?.AssetLoadingStarted
                .Where(x => x.Name.Contains(AtPart))
                .Subscribe(x =>
                {
                    _logger.Debug($"Module - Hud Stage - PrepareAssetSystem - RegisterToLoadFlow - Receive AssetLoadingStarted");
                    // var hudAssetName = x.HudAssetName;
                    var hudAssetName = $"Hud - Stage Assist/Hud Data";
                    LoadingAsset(hudAssetName);
                })
                .AddTo(_compositeDisposable);
#endif
        }
    }
}
