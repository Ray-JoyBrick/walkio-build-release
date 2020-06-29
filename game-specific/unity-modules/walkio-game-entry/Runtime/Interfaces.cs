namespace JoyBrick.Walkio.Game
{
    using System;
    using Unity.Entities;
    using UnityEngine;

    public interface IBootstrapAssistant
    {
        void SendCommand(string command);

        void HandleAfterEcsSetup();
    }
    
    public interface IBootstrapAssistable
    {
        GameObject RefGameObject { get; } 
        
        IObservable<int> CanStartInitialSetup { get; }
        
        void AddAssistant(IBootstrapAssistant assistant);
        
        //
        void ExecuteAction(int actionIndex, int times);
    }
}
