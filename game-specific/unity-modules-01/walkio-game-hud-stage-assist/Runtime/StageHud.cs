namespace JoyBrick.Walkio.Game.Hud.Stage.Assist
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;
    using UnityEngine.Assertions;

    //
    using GameCommand = Walkio.Game.Command;
    
    public partial class StageHud : MonoBehaviour
    {
        private static readonly UniRx.Diagnostics.Logger _logger = new UniRx.Diagnostics.Logger(nameof(StageHud));

        //
        private readonly CompositeDisposable _compositeDisposable = new CompositeDisposable();

        //
        public IObservable<GameCommand.ICommand> CommandStream => _notifyCommandGiven.AsObservable();
        private readonly Subject<GameCommand.ICommand> _notifyCommandGiven = new Subject<GameCommand.ICommand>();

        void Start()
        {
            //
            Assert.IsNotNull(addNeutralForceUnitButton);
            Assert.IsNotNull(addTeamForceUnitButton);

            //
            addNeutralForceUnitButton.OnClickAsObservable()
                .Subscribe(x =>
                {
                    //
                    // _notifyCommandGiven.OnNext();
                })
                .AddTo(_compositeDisposable);

            addTeamForceUnitButton.OnClickAsObservable()
                .Subscribe(x =>
                {
                    //
                    // _notifyCommandGiven.OnNext();
                })
                .AddTo(_compositeDisposable);
        }

        private void OnDestroy()
        {
            _compositeDisposable?.Dispose();
        }
    }
}
