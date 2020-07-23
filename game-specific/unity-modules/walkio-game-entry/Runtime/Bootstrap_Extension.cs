namespace JoyBrick.Walkio.Game
{
    using System.Collections.Generic;
    using System.Linq;
    using Pathfinding;
    using UniRx;
    using Unity.Entities;
    using Unity.Mathematics;
    using UnityEngine;

    //
#if WALKIO_EXTENSION
    using GameExtension = JoyBrick.Walkio.Game.Extension;
#endif

    public partial class Bootstrap

#if WALKIO_EXTENSION
        : GameExtension.IExtensionService
#endif

    {
#if PLAYMAKER
        private static void SetFsmVariableValue(PlayMakerFSM pmfsm, string variableName, GameObject inValue)
        {
            var commandServiceVariables =
                pmfsm.FsmVariables.GameObjectVariables.Where(x => string.CompareOrdinal(x.Name, variableName) == 0);

            commandServiceVariables.ToList()
                .ForEach(x =>
                {
                    x.Value = inValue;
                });
        }

#endif
        public void SetReferenceToExtension(GameObject inGO)
        {
#if PLAYMAKER
            var pmfsms = new List<PlayMakerFSM>();

            // Canvas itself
            var comps = inGO.GetComponents<PlayMakerFSM>();
            if (comps.Length > 0)
            {
                pmfsms.AddRange(comps);
            }

            // Views under Canvas
            foreach (Transform child in inGO.transform)
            {
                comps = child.GetComponents<PlayMakerFSM>();
                if (comps.Length > 0)
                {
                    pmfsms.AddRange(comps);
                }
            }

            pmfsms.ForEach(x => SetFsmVariableValue(x, "zz_Command Service", gameObject));
            pmfsms.ForEach(x => SetFsmVariableValue(x, "zz_Flow Service", gameObject));
            pmfsms.Clear();
#endif
        }

    }
}
