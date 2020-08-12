namespace JoyBrick.Walkio.Game.Creature.Template
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Creature Overview Repo Data", menuName = "Walkio/Game/Creature/Creature Overview Repo Data")]
    public class CreatureOverviewRepoData : ScriptableObject
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Collection")]
#endif
        public List<ScriptableObject> teamLeaderAssets;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Collection")]
#endif
        public List<ScriptableObject> teamMinionAssets;

    }
}
