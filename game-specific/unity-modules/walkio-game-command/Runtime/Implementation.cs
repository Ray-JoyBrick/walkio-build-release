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
        public bool Flag { get; set; }
    }

    public class ChangeToGameFlow : ICommand
    {
        public string FlowName { get; set; }
    }
    
    //
    public class CreateNeutralForceUnit : ICommand
    {
    }

    public enum TeamForceLeaderKind
    {
        NpcUse,
        PlayerUse
    }

    public class PlaceTeamForceLeader : ICommand
    {
        public TeamForceLeaderKind Kind { get; set; }
    }
    
    //
    public class CreateTeamForceUnit : ICommand
    {
        public int TeamId { get; set; }
        public int UnitKind { get; set; }
    }
}
