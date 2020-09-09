namespace JoyBrick.Walkio.Game.FlowControl.Template
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Used Flow Data", menuName = "Walkio/Game/Flow Control/Used Flow Data")]
    public class UsedFlowData : ScriptableObject
    {
        public List<GameObject> flowPrefabs;
    }
}
