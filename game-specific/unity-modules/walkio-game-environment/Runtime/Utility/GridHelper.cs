namespace JoyBrick.Walkio.Game.Environment.Utility
{
    using Unity.Mathematics;

    public static class GridHelper
    {
        public enum Direction : int
        {
            Up /* UpRight */ = 0,
            Right /* DownRight */ = 1,
            Down /* DownLeft */ = 2,
            Left /* UpLeft */ = 3
        }

        public struct NeighborOfDirection4
        {
            public int4 Value;
        }

        public struct NeighborOfDirection8
        {
            public int4 Value1;
            public int4 Value2;
        }

        private static int GridIndexOneSide(float gridCellSize, float position) =>
            (int)math.floor(position / gridCellSize);
        
        public static int GridCellIndex(
            int hGridCellCount, int vGridCellCount,
            float hGridCellSize, float vGridCellSize,
            float hPosition, float vPosition)
        {
            var hIndex = GridIndexOneSide(hGridCellSize, hPosition);
            var vIndex = GridIndexOneSide(vGridCellSize, vPosition);

            var index = (vIndex * hGridCellCount) + hIndex;

            return index;
        }

        public static int GridCellIndex(
            int2 gridCellCount, float2 gridCellSize, float2 position) =>
            GridCellIndex(
                gridCellCount.x, gridCellCount.y,
                gridCellSize.x, gridCellSize.y,
                position.x, position.y);
        
        public static int GridIndex1D(
            int hGridCellCount, int vGridCellCount,
            int hIndex, int vIndex) =>
            vIndex * hGridCellCount + hIndex;

        public static int2 GridIndex2D(
            int hGridCellCount, int vGridCellCount,
            int gridCellIndex)
        {
            var hIndex = gridCellIndex % hGridCellCount;
            var vIndex = (int)math.floor((float)gridCellIndex / (float)vGridCellCount);
            
            return new int2(hIndex, vIndex);
        }

        public static NeighborOfDirection4 FourDirectionNeighbors(
            int hGridCellCount, int vGridCellCount,
            int gridCellIndex)
        {
            var index2d = GridIndex2D(hGridCellCount, vGridCellCount, gridCellIndex);
            var leftIndex = (index2d.x - 1);
            var rightIndex = (index2d.x + 1);
            var upIndex = index2d.y + 1;
            var downIndex = index2d.y - 1;
            
            var left = (leftIndex > 0) ? GridIndex1D(hGridCellCount, vGridCellCount, leftIndex, index2d.y) : -1;
            var right = (rightIndex < hGridCellCount) ? GridIndex1D(hGridCellCount, vGridCellCount, rightIndex, index2d.y) : -1;
            var up = (upIndex < vGridCellCount) ? GridIndex1D(hGridCellCount, vGridCellCount, index2d.x, upIndex) : -1;
            var down = (downIndex > 0) ? GridIndex1D(hGridCellCount, vGridCellCount, index2d.x, downIndex) : -1;
            
            return new NeighborOfDirection4
            {
                Value = new int4(up, right, down, left)
            };
        }

        public static NeighborOfDirection8 EightDirectionNeighbors(
            int hGridCellCount, int vGridCellCount,
            int gridCellIndex)
        {
            var index2d = GridIndex2D(hGridCellCount, vGridCellCount, gridCellIndex);
            var leftIndex = (index2d.x - 1);
            var rightIndex = (index2d.x + 1);
            var upIndex = index2d.y + 1;
            var downIndex = index2d.y - 1;

            var left = (leftIndex > 0) ? GridIndex1D(hGridCellCount, vGridCellCount, leftIndex, index2d.y) : -1;
            var right = (rightIndex < hGridCellCount) ? GridIndex1D(hGridCellCount, vGridCellCount, rightIndex, index2d.y) : -1;
            var up = (upIndex < vGridCellCount) ? GridIndex1D(hGridCellCount, vGridCellCount, index2d.x, upIndex) : -1;
            var down = (downIndex > 0) ? GridIndex1D(hGridCellCount, vGridCellCount, index2d.x, downIndex) : -1;

            var downLeft = (downIndex > 0) && (leftIndex > 0) ?
                GridIndex1D(hGridCellCount, vGridCellCount, leftIndex, downIndex) : -1;
            var upLeft = (upIndex < vGridCellCount) && (leftIndex > 0) ?
                GridIndex1D(hGridCellCount, vGridCellCount, leftIndex, upIndex) : -1;
            var upRight = ((rightIndex < hGridCellCount) && (upIndex < vGridCellCount)) ?
                GridIndex1D(hGridCellCount, vGridCellCount, rightIndex, upIndex) : -1;
            var downRight = (downIndex > 0) && (rightIndex < hGridCellCount) ?
                GridIndex1D(hGridCellCount, vGridCellCount, rightIndex, downIndex) : -1;

            return new NeighborOfDirection8
            {
                Value1 = new int4(up, right, down, left),
                Value2 = new int4(upRight, downRight, downLeft, upLeft)
            };
        }
    }
}