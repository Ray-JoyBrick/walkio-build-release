namespace JoyBrick.Walkio.Game
{
    using System;
    using UniRx;
    using UnityEngine;
    using Unity.Entities;

    // //
    // using GameCommon = TBG.Game.Common;
    //
    // public partial class Bootstrap :
    //     IFlowControl
    // {
    //     //
    //     private readonly Subject<GameCommon.FlowControlContext> _notifyAssetLoadingStarted =
    //         new Subject<GameCommon.FlowControlContext>();
    //     public IObservable<FlowControlContext> AssetLoadingStarted => _notifyAssetLoadingStarted.AsObservable();
    //
    //     //
    //     private readonly Subject<GameCommon.FlowControlContext> _notifyAssetLoadingDone =
    //         new Subject<GameCommon.FlowControlContext>();
    //     public IObservable<FlowControlContext> AssetLoadingDone => _notifyAssetLoadingDone.AsObservable();
    //
    //     //
    //     private readonly Subject<GameCommon.FlowControlContext> _notifyAssetSettingStarted =
    //         new Subject<GameCommon.FlowControlContext>();
    //     public IObservable<FlowControlContext> AssetSettingStarted => _notifyAssetSettingStarted.AsObservable();
    //
    //     //
    //     private readonly Subject<GameCommon.FlowControlContext> _notifyAssetSettingDone =
    //         new Subject<GameCommon.FlowControlContext>();
    //     public IObservable<FlowControlContext> AssetSettingDone => _notifyAssetSettingDone.AsObservable();
    //
    //     public void FinishIndividualLoadingAsset(FlowControlContext context)
    //     {
    //     }
    //
    //     public void FinishIndividualSettingAsset(FlowControlContext context)
    //     {
    //     }
    // }
}
