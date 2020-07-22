namespace JoyBrick.Walkio.Game.Move.FlowField.Assist.Template
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class Group
    {
        public Color color;
    }

    [CreateAssetMenu(fileName = "Flow Field World Data", menuName = "Walkio/Game/Flow Field Assist/Flow Field World Data")]
    public class FlowFieldWorldData : ScriptableObject
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Time Section")]
#endif
        public float temporaryPointShowTime;

        public List<Group> groups;
    }
}
