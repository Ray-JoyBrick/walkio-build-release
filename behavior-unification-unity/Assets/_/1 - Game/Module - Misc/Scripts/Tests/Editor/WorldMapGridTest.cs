namespace JoyBrick.Walkio.Game.Environment.Utility.EditorPart.Tests
{
    using NUnit.Framework;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.TestTools;

    using GameEnvironmentUtility = JoyBrick.Walkio.Game.Environment.Utility; 

    public class WorldMapGridTest
    {
        [Test]
        public void GetGridCellIndexFromPosition()
        {
            // arrange
            var gridCellCount = new int2(10, 10);
            var gridCellSize = new float2(1.0f, 1.0f);
            var position = new float3(4.0f, 0, 1.9f);
        
            // act
            var gridCellIndex =
                GameEnvironmentUtility.WorldMapGridHelper.PositionToGridCellIndex2D(
                    gridCellCount,
                    gridCellSize,
                    new float2(position.x, position.z));
        
            // assert
            var expected = new int2(4, 1);
            Assert.AreEqual(expected, gridCellIndex);
        }

        [Test]
        public void GetOutBoundIndex2DFromPosition()
        {
            // arrange
            var gridCellCount = new int2(10, 10);
            var gridCellSize = new float2(1.0f, 1.0f);
            var position1 = new float3(3.5f, 0, 6.8f);
            var position2 = new float3(14.0f, 0, 1.9f);
            var position3 = new float3(4.7f, 0, 10.9f);
            var position4 = new float3(29.1f, 0, 10.0f);

            // act
            var gridCellIndex1 =
                GameEnvironmentUtility.WorldMapGridHelper.PositionToGridCellIndex2D(
                    gridCellCount,
                    gridCellSize,
                    new Vector2(position1.x, position1.z));

            var gridCellIndex2 =
                GameEnvironmentUtility.WorldMapGridHelper.PositionToGridCellIndex2D(
                    gridCellCount,
                    gridCellSize,
                    new Vector2(position2.x, position2.z));

            var gridCellIndex3 =
                GameEnvironmentUtility.WorldMapGridHelper.PositionToGridCellIndex2D(
                    gridCellCount,
                    gridCellSize,
                    new Vector2(position3.x, position3.z));

            var gridCellIndex4 =
                GameEnvironmentUtility.WorldMapGridHelper.PositionToGridCellIndex2D(
                    gridCellCount,
                    gridCellSize,
                    new Vector2(position4.x, position4.z));

            // assert
            var expected1 = new int2(3, 6);
            var expected2 = new int2(-1, 1);
            var expected3 = new int2(4, -1);
            var expected4 = new int2(-1, -1);

            Assert.AreEqual(expected1, gridCellIndex1);
            Assert.AreEqual(expected2, gridCellIndex2);
            Assert.AreEqual(expected3, gridCellIndex3);
            Assert.AreEqual(expected4, gridCellIndex4);
        }
        
        [Test]
        public void GetOutBoundIndex1DFromPosition()
        {
            // arrange
            var gridCellCount = new int2(10, 10);
            var gridCellSize = new float2(1.0f, 1.0f);
            var position1 = new float3(3.5f, 0, 6.8f);
            var position2 = new float3(14.0f, 0, 1.9f);
            var position3 = new float3(4.7f, 0, 10.9f);
            var position4 = new float3(29.1f, 0, 10.0f);

            // act
            var gridCellIndex1 =
                GameEnvironmentUtility.WorldMapGridHelper.PositionToGridCellIndex1D(
                    gridCellCount,
                    gridCellSize,
                    new float2(position1.x, position1.z));

            var gridCellIndex2 =
                GameEnvironmentUtility.WorldMapGridHelper.PositionToGridCellIndex1D(
                    gridCellCount,
                    gridCellSize,
                    new float2(position2.x, position2.z));

            var gridCellIndex3 =
                GameEnvironmentUtility.WorldMapGridHelper.PositionToGridCellIndex1D(
                    gridCellCount,
                    gridCellSize,
                    new float2(position3.x, position3.z));

            var gridCellIndex4 =
                GameEnvironmentUtility.WorldMapGridHelper.PositionToGridCellIndex1D(
                    gridCellCount,
                    gridCellSize,
                    new float2(position4.x, position4.z));

            // assert
            var expected1 = 63;
            var expected2 = -1;
            var expected3 = -1;
            var expected4 = -1;

            Assert.AreEqual(expected1, gridCellIndex1);
            Assert.AreEqual(expected2, gridCellIndex2);
            Assert.AreEqual(expected3, gridCellIndex3);
            Assert.AreEqual(expected4, gridCellIndex4);
        }
        
        [Test]
        public void GetPositionFromGridCellIndex()
        {
            // arrange
            var gridCellCount = new int2(10, 10);
            var gridCellSize = new float2(1.0f, 1.0f);
            var gridCellIndex = new int2(3, 5);
        
            // act
            var position =
                GameEnvironmentUtility.WorldMapGridHelper.GridCellIndexToPosition2D(
                    gridCellCount,
                    gridCellSize,
                    gridCellIndex);
        
            // assert
            var expected = new float2(3.5f, 5.5f);
            Assert.LessOrEqual((expected.x - position.x), Mathf.Epsilon);
            Assert.LessOrEqual((expected.y - position.y), Mathf.Epsilon);
        }
    }
}
