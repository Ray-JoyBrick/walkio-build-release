namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
    using UnityEngine;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;

    public partial class Bootstrap :
        GameCommon.ISceneProvider
        
    {
        public GameObject neutralUnitPrefab;
        public List<GameObject> teamUnitPrefabs;

        public Camera camera;
        public List<Mesh> unitMeshs;
        public List<Material> unitMaterials;

        // private void CreateWorld()
        // {
        //     var world = new World("Render");
        //     var systemTypes = new List<System.Type>();
        //     
        //     DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, systemTypes);
        //     
        //     ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);
        //
        //     ScriptBehaviourUpdateOrder.SetPlayerLoop(PlayerLoop.GetDefaultPlayerLoop());
        //
        //     foreach (var w in World.All)
        //     {
        //         ScriptBehaviourUpdateOrder.UpdatePlayerLoop(w, ScriptBehaviourUpdateOrder.CurrentPlayerLoop);
        //     }
        // }        
        public Camera SceneCamera => camera;
    }
}
