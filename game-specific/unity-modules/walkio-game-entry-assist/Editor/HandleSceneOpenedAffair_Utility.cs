namespace JoyBrick.Walkio.Game.Editor.Assist
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    using GameEnvironment = JoyBrick.Walkio.Game.Environment;

    // TODO: Rename, this class name brings only confusion
    public static partial class HandleSceneOpenedAffair
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
