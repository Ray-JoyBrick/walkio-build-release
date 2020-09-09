namespace JoyBrick.Walkio.Game.Creature.Template
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Creature Repo Data", menuName = "Walkio/Game/Creature/Creature Repo Data")]
    public class CreatureRepoData : ScriptableObject
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Collection")]
#endif
        public List<ScriptableObject> teamLeaderNpcAssets;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Collection")]
#endif
        public List<ScriptableObject> teamLeaderPlayerAssets;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Collection")]
#endif
        public List<ScriptableObject> teamMinionAssets;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Collection")]
#endif
        public List<ScriptableObject> neutralMinionAssetss;
    }
}
