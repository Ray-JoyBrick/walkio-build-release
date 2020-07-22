namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public partial class Bootstrap :
        GameCommon.IGizmoService
    {
        // Delegate to assistants to handle
        public void AddEntityToGameObject(Entity entity)
        {
            _assistants.ForEach(x => x.AddEntityToGameObject(entity));
        }

        public void RemoveEntityFromGameObject(Entity entity)
        {
            _assistants.ForEach(x => x.RemoveEntityFromGameObject(entity));
        }
    }
}
