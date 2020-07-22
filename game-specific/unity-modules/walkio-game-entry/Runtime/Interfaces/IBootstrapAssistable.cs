namespace JoyBrick.Walkio.Game
{
    using System;
    using UnityEngine;

    public interface IBootstrapAssistable
    {
        GameObject RefGameObject { get; }

        IObservable<int> SetupBeforeEcs { get; }
        IObservable<int> SetupAfterEcs { get; }

        void AddAssistant(IBootstrapAssistant assistant);
    }
}
