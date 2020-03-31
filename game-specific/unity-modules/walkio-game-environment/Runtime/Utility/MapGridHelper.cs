// namespace JoyBrick.Walkio.Game.Environment.Utility
// {
//     using Unity.Mathematics;
//     using UnityEngine;
//
//     public static class MapGridHelper
//     {
//         private static int GridIndex1D(float gridCellSize, float position) =>
//             (int)math.floor(position / gridCellSize);
//         
//         public static int GridIndex(
//             int hGridCellCount, int vGridCellCount,
//             float hGridCellSize, float vGridCellSize,
//             float hPosition, float vPosition)
//         {
//             var hIndex = GridIndex1D(hGridCellSize, hPosition);
//             var vIndex = GridIndex1D(vGridCellSize, vPosition);
//
//             var index = (vIndex * hGridCellCount) + hIndex;
//
//             return index;
//         }
//     }
// }
