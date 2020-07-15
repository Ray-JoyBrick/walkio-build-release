namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;

    //
    using GameLevelCommon = JoyBrick.Walkio.Game.Level.Common;

    public partial class Bootstrap :
        GameLevelCommon.IGridWorldProvider
    {
        private Entity _gridWorldEntity;
        public Entity GridWorldEntity
        {
            get => _gridWorldEntity;
            set => _gridWorldEntity = value;
        }
    }
}
