namespace JoyBrick.Walkio.Game.Creature.Template
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class TeamLeaderOverview
    {
        public int id;
        public int titleId;

        public override string ToString()
        {
            var desc = $"id: {id} titleId: {titleId}";
            return desc;
        }
    }

    [System.Serializable]
    public class TeamMinionOverview
    {
        public int id;
        public int titleId;

        public override string ToString()
        {
            var desc = $"id: {id} titleId: {titleId}";
            return desc;
        }
    }

    [CreateAssetMenu(fileName = "Creature Overview Repo Data", menuName = "Walkio/Game/Creature/Creature Overview Repo Data")]
    public class CreatureOverviewRepoData : ScriptableObject
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Collection")]
#endif
        public List<TeamLeaderOverview> teamLeaderOverviews;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Collection")]
#endif
        public List<TeamMinionOverview> teamMinionOverviews;

    }
}
