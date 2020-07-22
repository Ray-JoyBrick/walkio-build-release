namespace JoyBrick.Walkio.Game.Move.FlowField.Template
{
    using UnityEngine;

    [CreateAssetMenu(fileName = "Flow Field World Data", menuName = "Walkio/Game/Flow Field/Flow Field World Data")]
    public class FlowFieldWorldData : ScriptableObject
    {
        public Vector2Int tileCellCount;
        public Vector2 tileCellSize;

        public Vector3 originalPosition;
        public Vector3 positionOffset;
    }
}
