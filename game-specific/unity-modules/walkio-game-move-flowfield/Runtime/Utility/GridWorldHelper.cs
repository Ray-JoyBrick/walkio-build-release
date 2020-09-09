namespace JoyBrick.Walkio.Game.Move.FlowField.Utility
{
    using System.Collections.Generic;
    using System.Linq;
    using Unity.Mathematics;
    using UnityEngine;

    public static partial class GridWorldHelper
    {
        //
        private static int PositionToGridCellIndex1D(float gridCellSize, float position) =>
            (int)math.floor(position / gridCellSize);

        private static float GridCellIndexToPosition1D(float gridCellSize, int gridCellIndex) =>
            (gridCellIndex * gridCellSize) + (gridCellSize * 0.5f);

        public static int2 PositionToGridCellIndex2D(int2 gridCellCount, float2 gridCellSize,
            float2 position)
        {
            var convert = new int2(
                PositionToGridCellIndex1D(gridCellSize.x, position.x),
                PositionToGridCellIndex1D(gridCellSize.y, position.y));

            var result = new int2(
                (convert.x < gridCellCount.x) ? convert.x : -1,
                (convert.y < gridCellCount.y) ? convert.y : -1);

            return result;
        }

        public static int PositionToGridCellIndex1D(int2 gridCellCount, float2 gridCellSize,
            float2 position)
        {
            var index2D = PositionToGridCellIndex2D(gridCellCount, gridCellSize, position);

            var result = (index2D.x < 0 || index2D.y < 0) ? -1 : ((index2D.y * gridCellCount.x) + index2D.x);

            return result;
        }
    }
}
