namespace JoyBrick.Walkio.Game
{
    using System;
    using UnityEngine;

    public interface IBootstrapAssistant
    {
        
    }
    
    public interface IBootstrapAssistable
    {
        GameObject RefGameObject { get; } 
        
        IObservable<int> CanStartInitialSetup { get; }
        
        void AddAssistant(IBootstrapAssistant assistant);
    }
}
