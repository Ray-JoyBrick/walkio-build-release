namespace JoyBrick.Walkio.Game
{
    using System;
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;
    using Unity.Entities;

    public partial class Bootstrap :
        IBootstrapAssistable
    {
        private readonly List<IBootstrapAssistant> _assistants = new List<IBootstrapAssistant>();

        public GameObject RefGameObject => this.gameObject;

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
    }
}
