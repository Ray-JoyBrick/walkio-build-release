namespace JoyBrick.Walkio.Game.FlowControl.System
{
    using System;
    using UniRx;

    public class FlowControlContext
    {
        //
        public string Name { get; set; }

        // Used across several divisions
        public string HudAssetName { get; set; }
        
        // Only used in Stage
        public string LevelAssetName { get; set; }
        public string SpecificLevelName { get; set; }

        public override string ToString()
        {
            var desc = $"Name: {Name}";

            return desc;
        }
    }

    // The flow is for each conceptual division(part) such as
    // App
    // Preparation
    // Stage
    public interface IFlowControl
    {
        // IObservable<FlowControlContext> AssetLoadingStarted { get; }
        IObservable<FlowControlContext> LoadingAsset { get; }
        // IObservable<FlowControlContext> AssetLoadingDone { get; }
        IObservable<FlowControlContext> DoneLoadingAsset { get; }

        // IObservable<FlowControlContext> AssetSettingStarted { get; }
        IObservable<FlowControlContext> SettingAsset { get; }
        // IObservable<FlowControlContext> AssetSettingDone { get; }
        IObservable<FlowControlContext> DoneSettingAsset { get; }

        IObservable<FlowControlContext> AllDoneSettingAsset { get; }

        IObservable<FlowControlContext> CleaningAsset { get; }
        
        //
        void StartLoadingAsset(string flowName);

        //
        // void FinishIndividualLoadingAsset(FlowControlContext context);
        void FinishLoadingAsset(FlowControlContext context);
        void StartSetting(FlowControlContext context);
        // void FinishIndividualSettingAsset(FlowControlContext context);
        void FinishSetting(FlowControlContext context);

        void FinishAllSetting(FlowControlContext context);
        
        //
        void StartCleaningAsset(string flowName);
    }
}