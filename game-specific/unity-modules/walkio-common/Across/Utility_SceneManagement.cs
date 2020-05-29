namespace JoyBrick.Walkio.Common
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine.SceneManagement;

    public static partial class Utility
    {
        public static T GetComponentAtScene<T>(Scene scene)
            where T : UnityEngine.Component
        {
            T comp = default;

            if (!scene.IsValid()) return comp;

            var foundGameObjects =
                scene.GetRootGameObjects()
                    .Where(x => x.GetComponent<T>() != null)
                    .ToList();

            if (!foundGameObjects.Any()) return comp;

            var foundGameObject = foundGameObjects.First();
            comp = foundGameObject.GetComponent<T>();

            return comp;
        }

        public static IEnumerable<T> GetComponentsAtScene<T>(Scene scene)
            where T : UnityEngine.Component
        {
            var comps = new List<T>();

            if (!scene.IsValid()) return comps;

            var foundGameObjects =
                scene.GetRootGameObjects()
                    .Where(x => x.GetComponent<T>() != null)
                    .ToList();

            if (!foundGameObjects.Any()) return comps;

            var castedComps = foundGameObjects.Select(x => x.GetComponent<T>());
            comps.AddRange(castedComps);

            return comps;
        }
    }
}
