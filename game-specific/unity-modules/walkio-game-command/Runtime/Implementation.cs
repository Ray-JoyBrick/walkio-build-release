namespace JoyBrick.Walkio.Game.Command
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using UniRx;
    using Unity.Entities;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public class ActivateLoadingViewCommand : ICommand
    {
        public bool flag;
    }
}
