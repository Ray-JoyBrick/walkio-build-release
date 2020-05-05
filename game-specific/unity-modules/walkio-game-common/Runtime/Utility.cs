namespace JoyBrick.Walkio.Game.Common
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public static partial class Utility
    {
        private static T GetComponentAtScene<T>(Scene scene) where T : UnityEngine.Component
        {
            T comp = default;

            if (!scene.IsValid()) return comp;

            var foundGOs =
                scene.GetRootGameObjects()
                    .Where(x => x.GetComponent<T>() != null)
                    .ToList();

            if (foundGOs != null && foundGOs.Any())
            {
                var foundGO = foundGOs.First();
                comp = foundGO.GetComponent<T>();
            }

            return comp;
        }        
    }
}
