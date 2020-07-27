namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
    using System.Linq;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    //
    using GameService = JoyBrick.Walkio.Game.Service;

    //
    public partial class Bootstrap :
        GameService.AStar.IAStarService
    {
    }
}
