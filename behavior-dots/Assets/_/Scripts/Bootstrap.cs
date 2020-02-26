namespace JoyBrick.Walkio.Game.Main
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using Unity.Entities;
    using UnityEngine;

    public class CustomBootstrap : ICustomBootstrap
    {
        public bool Initialize(string defaultWorldName)
        {
            Debug.Log($"CustomBootstrap - Initialize: {defaultWorldName}");
            var world = new World(defaultWorldName);
            World.DefaultGameObjectInjectionWorld = world;
            var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
 
            DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, systems);
            ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);

            return true;
        }
    }

    public class Bootstrap :
        MonoBehaviour//,
        // ICustomBootstrap
    {
        void Start()
        {
            // Initialize("Client");
            
            Debug.Log($"Bootstrap - Start");

            var customBootstrap = new CustomBootstrap();
            customBootstrap.Initialize("Client");
            
            var clientWorlds =
                World.AllWorlds
                    .Where(w => string.Compare(w.Name, "Client", StringComparison.Ordinal) == 0)
                    .ToList();
            if (clientWorlds.Any())
            {
                var clientWorld = clientWorlds.First();
                var playerInputSystem = clientWorld.GetOrCreateSystem<PlayerInputSystem>();
            }
        }

        // public bool Initialize(string defaultWorldName)
        // {
        //     //throw new System.NotImplementedException();
        //
        //     Debug.Log($"Bootstrap - Initialize: {defaultWorldName}");
        //     var world = new World("Client");
        //     World.DefaultGameObjectInjectionWorld = world;
        //     var systems = DefaultWorldInitialization.GetAllSystems(WorldSystemFilterFlags.Default);
        //
        //     DefaultWorldInitialization.AddSystemsToRootLevelSystemGroups(world, systems);
        //     ScriptBehaviourUpdateOrder.UpdatePlayerLoop(world);
        //
        //     return true;
        //     
        //     // var clientWorlds =
        //     //     World.AllWorlds
        //     //         .Where(w => string.Compare(w.Name, "Client", StringComparison.Ordinal) == 0)
        //     //         .ToList();
        //     // if (clientWorlds.Any())
        //     // {
        //     //     var clientWorld = clientWorlds.First();
        //     //     var playerInputSystem = clientWorld.GetOrCreateSystem<PlayerInputSystem>();
        //     // }
        //
        //     return true;
        // }
    }
}
