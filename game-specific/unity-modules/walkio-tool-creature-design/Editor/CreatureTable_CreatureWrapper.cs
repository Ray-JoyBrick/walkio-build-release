namespace JoyBrick.Walkio.Build.CreatureDesign.EditorPart
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    using UnityEditor;
    using UnityEditor.SceneManagement;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    //
    using GameCommon = JoyBrick.Walkio.Game.Common;
    using Object = UnityEngine.Object;
    // //
    // using GameLevel = JoyBrick.Walkio.Game.Level;
    // using BuildLevelDesignExport = JoyBrick.Walkio.Build.LevelDesignExport;

    public partial class CreatureTable
    {
        private class CreatureWrapper
        {
            // Level is a ScriptableObject and would render a unity object
            // field if drawn in the inspector, which is not what we want.

            public Creature Creature { get; }

            public CreatureWrapper(Creature creature)
            {
                this.Creature = creature;
            }

#if ODIN_INSPECTOR
            [TableColumnWidth(50, false)]
            [ShowInInspector, PreviewField(45, ObjectFieldAlignment.Center)]
#endif
            public Texture Icon
            {
                get => this.Creature.icon;
                set
                {
                    this.Creature.icon = value;
                    EditorUtility.SetDirty(this.Creature);
                }
            }

#if ODIN_INSPECTOR
            [TableColumnWidth(120)]
            [ShowInInspector]
#endif
            public string Title
            {
                get => this.Creature.title;
                set
                {
                    this.Creature.title = value;
                    EditorUtility.SetDirty(this.Creature);
                }
            }

#if ODIN_INSPECTOR
            [Button("Generate Scene"), GUIColor(0, 1, 0)]
            [ShowInInspector]
#endif
            public void GenerateExportScene()
            {
            }

#if ODIN_INSPECTOR
            [Button("Generate"), GUIColor(0, 1, 0)]
            [ShowInInspector]
#endif
            public void GenerateExportAsset()
            {
            }
        }
    }
}
