namespace JoyBrick.Walkio.Game.Assist
{
    using System;
    using System.Linq;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.Events;
    using UnityEngine.SceneManagement;

    public partial class Bootstrap
    {
        public void SendCommand(string commandName)
        {
            if (String.IsNullOrEmpty(commandName)) return;
            
            Debug.Log($"Bootstrap Assist - SendCommand - commandName: {commandName}");
            
            if (String.CompareOrdinal(commandName, "Create Test Neutral Unit") == 0)
            {
                // _assistable.ExecuteAction(10, 100);
            }
            else if (String.CompareOrdinal(commandName, "Create Test Pickup") == 0)
            {
            }
        }
    }
}
