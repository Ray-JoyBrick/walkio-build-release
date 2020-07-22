namespace JoyBrick.Walkio.Game.StageFlowControl
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Control Flow Data", menuName = "Walkio/Game/Stage Flow Control/Control Flow Data")]
    public class ControlFlowData : ScriptableObject
    {
        public List<GameObject> flowPrefabs;
    }
}
