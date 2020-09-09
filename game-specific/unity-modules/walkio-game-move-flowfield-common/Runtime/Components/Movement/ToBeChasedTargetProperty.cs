﻿namespace JoyBrick.Walkio.Game.Move.FlowField
{
    using Unity.Entities;
    using Unity.Mathematics;

    [RequireComponentTag(typeof(ToBeChasedTarget))]
    public struct ToBeChasedTargetProperty : IComponentData
    {
        //
        public int BelongToGroup;

        //
        public bool Initialized;

        //
        public int2 AtTileIndex;
        public int2 AtTileCellIndex;
        public Entity AtTileEntity;

        //
        public Entity LeadingToSetEntity;
    }
}
