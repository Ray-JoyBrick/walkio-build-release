﻿namespace JoyBrick.Walkio.Game.Creature.Template
{
    using System.Collections.Generic;
    using UnityEngine;

    [CreateAssetMenu(fileName = "Creature Data", menuName = "Walkio/Game/Creature/Creature Data")]
    public class CreatureData : ScriptableObject
    {
        //
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Basic")]
#endif
        public GameObject avatarPrefab;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Vertex Animation")]
#endif
        public Mesh mesh;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.BoxGroup("Vertex Animation")]
#endif
        public Material material;

    }
}
