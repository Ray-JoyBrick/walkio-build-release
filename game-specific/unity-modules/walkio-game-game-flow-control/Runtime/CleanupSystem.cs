namespace JoyBrick.Walkio.Game.GameFlowControl
{
    using System;
    using Command;
    using UniRx;
    using Unity.Entities;

    using GameCommon = JoyBrick.Walkio.Game.Common;

    public class CleanupSystem : SystemBase
    {
        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        // Should separate into
        // Flow state
        // Command
        public ICommandService CommandService { get; set; }
        
        public GameCommon.IFlowControl FlowControl { get; set; }

        public void Construct()
        {
            // FlowControl.CleaningAsset
            //     .Where(x => x.Name.Contains("Preparation"))
            //     .Buffer(1)
            //     .Subscribe(x =>
            //     {
            //     })
            //     .AddTo(_compositeDisposable);         
            
        }

        protected override void OnUpdate() {}
    }
}
