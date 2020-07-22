namespace JoyBrick.Walkio.Game.FlowControl
{
    using System;
    using UnityEngine;

    public class FlowControlContext
    {
        //
        public string Name { get; set; }
        
        //
        public string HudAssetName { get; set; }
        
        //
        public string LevelAssetName { get; set; }
        public string SpecificLevelName { get; set; }

        //
        public override string ToString()
        {
            var desc = $"Name: {Name}";

            return desc;
        }
    }

    public interface IFlowControl
    {
        //
        ScriptableObject FlowControlData { get; }
        
        //
        IObservable<FlowControlContext> AssetLoadingStarted { get; }
        
        IObservable<FlowControlContext> IndividualAssetLoadingFinished { get; }
        
        // IObservable<FlowControlContext> AssetLoadingDone { get; }

        //
        IObservable<FlowControlContext> AssetSettingStarted { get; }

        IObservable<FlowControlContext> IndividualAssetSettingFinished { get; }

        // IObservable<FlowControlContext> AssetSettingDone { get; }
        
        //
        IObservable<FlowControlContext> FlowReadyToStart { get; }

        //
        void StartLoadingAsset(FlowControlContext context);
        
        void FinishIndividualLoadingAsset(FlowControlContext context);
        void FinishIndividualSettingAsset(FlowControlContext context);

        void AllAssetLoadingDone(FlowControlContext context);
        void AllAssetSettingDone(FlowControlContext context);
    }
}
