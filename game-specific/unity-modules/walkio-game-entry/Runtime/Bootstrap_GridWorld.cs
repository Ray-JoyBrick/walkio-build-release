namespace JoyBrick.Walkio.Game
{
    using System;
    using System.Collections.Generic;
    using UniRx;
    using UnityEngine;
    using Unity.Entities;

    //
    using GameLevel = JoyBrick.Walkio.Game.Level;

    public partial class Bootstrap :
        GameLevel.IGridWorldProvider
    {
        public Entity GridWorldEntity { get; set; }
    }
}
