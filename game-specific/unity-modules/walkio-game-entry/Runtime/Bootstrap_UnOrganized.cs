namespace JoyBrick.Walkio.Game
{
    using UnityEngine;

    public partial class Bootstrap
    {
        public GameObject neutralUnitPrefab;
        public GameObject teamUnitPrefab;

        public Camera camera;
        public Mesh unitMesh;
        public Material unitMaterial;

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
    }
}
