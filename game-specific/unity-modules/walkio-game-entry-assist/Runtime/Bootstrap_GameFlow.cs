namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using System.Linq;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public partial class Bootstrap
    {
        private void StartGameFlow()
        {
            _logger.Debug($"Bootstrap Assist - StartGameFlow");

#if CREATURE_DESIGN_PROJECT
            SignalStartLoadingAssetForStage();
#endif
        }

#if CREATURE_DESIGN_PROJECT

        private void SignalStartLoadingAssetForStage()
        {
            Observable.Timer(System.TimeSpan.FromMilliseconds(500))
                .Subscribe(_ =>
                {
                    var flowControl = _assistable.RefGameObject.GetComponent<GameCommon.IFlowControl>();
                    flowControl.StartLoadingAsset("Stage");
                })
                .AddTo(_compositeDisposable);
        }

#endif
    }
}
