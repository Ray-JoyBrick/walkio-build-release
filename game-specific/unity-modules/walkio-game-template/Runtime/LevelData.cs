namespace JoyBrick.Walkio.Game.Template
{
    using UnityEngine;

    
    [CreateAssetMenu(fileName = "Level Data", menuName = "Walkio/Game/Template/Level Data")]
    public class LevelData : ScriptableObject
    {
        public int Id;
        public GameObject environmentPrefab;
        public TextAsset pathfindingData;
        public Texture gridTexture;
    }
}
