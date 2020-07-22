namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
    using Unity.Entities;
    using UnityEngine;

    //
    using GameMoveFlowFieldCommon = JoyBrick.Walkio.Game.Move.FlowField.Common;

    public partial class Bootstrap :
        GameMoveFlowFieldCommon.IFlowFieldWorldProvider
    {
        private Entity _flowFieldWorldEntity;
        public Entity FlowFieldWorldEntity
        {
            get => _flowFieldWorldEntity;
            set => _flowFieldWorldEntity = value;
        }
        
        public GameObject FlowFieldTileBlobAssetAuthoringPrefab { get; set; }
    }
}
