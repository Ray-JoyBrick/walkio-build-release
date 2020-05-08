namespace JoyBrick.Walkio.Game
{
    using System;
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;

    using GameCommand = JoyBrick.Walkio.Game.Command;
#if COMPLETE_PROJECT || BEHAVIOR_PROJECT
    using GameExtension = JoyBrick.Walkio.Game.Extension;
#endif

    public partial class Bootstrap :
        IBootstrapAssistable
    {
        public GameObject RefGameObject => this.gameObject; 
        
        private readonly List<IBootstrapAssistant> _assistants = new List<IBootstrapAssistant>();

        public IObservable<int> CanStartInitialSetup => _notifyCanStartInitialSetup.AsObservable();
        private readonly Subject<int> _notifyCanStartInitialSetup = new Subject<int>();

        public void AddAssistant(IBootstrapAssistant assistant)
        {
            var existed = _assistants.Exists(x => x == assistant);
            if (!existed)
            {
                _assistants.Add(assistant);
            }
        }

        public void ExecuteAction(int actionIndex, int times)
        {
            if (actionIndex == 10)
            {
                for (var i = 0; i < times; ++i)
                {
                    _notifyCommand.OnNext(new GameCommand.CreateNeutralForceUnit
                    {
                    });
                }
            }
        }
    }
}
