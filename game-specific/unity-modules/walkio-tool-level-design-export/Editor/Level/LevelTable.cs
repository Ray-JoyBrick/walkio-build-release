// namespace JoyBrick.Walkio.Build.LevelDesignExport.EditorPart
// {
//     using System.Collections.Generic;
//     using System.Linq;
// #if ODIN_INSPECTOR
//     using Sirenix.OdinInspector;
// #endif
//     using UnityEditor;
//     using UnityEngine;
//
//     public partial class LevelTable
//     {
//         [TableList(IsReadOnly = true, AlwaysExpanded = true), ShowInInspector]
//         private readonly List<LevelWrapper> _allLevels;
//
//         public Level this[int index] => _allLevels[index].Level;
//
//         public LevelTable(IEnumerable<Level> levels)
//         {
//             if (levels != null)
//             {
//                 _allLevels =
//                     levels
//                         .Select(x =>
//                         {
//                             if (x == null) return null;
//                             return new LevelWrapper(x);
//                         })
//                         .Where(x => x != null)
//                         .ToList();
//             }
//             else
//             {
//                 _allLevels = new List<LevelWrapper>();
//             }
//         }
//     }
// }
