namespace JoyBrick.Walkio.Game.Level.Template
{
    using System.Collections.Generic;
    using UnityEngine;

    [System.Serializable]
    public class AreaItem
    {
        public Color32 areaColor;
        public string tagName;
        public int index;
    }

    [CreateAssetMenu(fileName = "Level Data", menuName = "Walkio/Game/Level/Level Data")]
    public class LevelData : ScriptableObject
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Lookup")]
#endif
        public List<AreaItem> areaLookup;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Basic")]
#endif
        public Vector2Int gridWorldTileCount;
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Basic")]
#endif
        public Vector2Int gridWorldTielCellCount;
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Basic")]
#endif
        public Vector2 gridWorldCellSize;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Obstacle")]
#endif
        public List<Texture2D> subLevelImages;
    }
}
