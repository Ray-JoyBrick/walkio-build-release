namespace JoyBrick.Walkio.Game.Move.FlowField.Utility.EditorPart.Tests
{
    using NUnit.Framework;
    using Unity.Collections;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.TestTools;

    public class FlowFieldTileTest
    {
        [Test]
        public void GetTileCountFromGrid2D()
        {
            // arrange
            var gridCellCount = new int2(64, 32);
            var gridCellSize = new float2(1.0f, 1.0f);
            var tileCellCount = new int2(10, 10);
            var tileCellSize = new float2(1.0f, 1.0f);

            // act
            var tileCount2D =
                FlowFieldTileHelper.GetTileCountFromGrid2D(
                    gridCellCount,
                    gridCellSize,
                    tileCellCount,
                    tileCellSize);

            // assert
            var expected = new int2(7, 4);
            Assert.AreEqual(expected, tileCount2D);
        }

        [Test]
        public void GetTileCountFromGrid1D()
        {
            // arrange
            var gridCellCount = new int2(64, 32);
            var gridCellSize = new float2(1.0f, 1.0f);
            var tileCellCount = new int2(10, 10);
            var tileCellSize = new float2(1.0f, 1.0f);

            // act
            var tileCount =
                FlowFieldTileHelper.GetTileCountFromGrid1D(
                    gridCellCount,
                    gridCellSize,
                    tileCellCount,
                    tileCellSize);

            // assert
            var expected = 28;
            Assert.AreEqual(tileCount, expected);
        }

        [Test]
        public void GetTileIndexFromPosition2D()
        {
            // arrange
            var gridCellCount = new int2(64, 32);
            var gridCellSize = new float2(1.0f, 1.0f);
            var tileCellCount = new int2(10, 10);
            var tileCellSize = new float2(1.0f, 1.0f);
            var position = new float3(14.5f, 0, 20.1f);

            // act
            var tileIndex =
                FlowFieldTileHelper.PositionToTileIndexAtGrid2D(
                    gridCellCount,
                    gridCellSize,
                    tileCellCount,
                    tileCellSize,
                    new float2(position.x, position.z));

            // assert
            var expected = new int2(1, 2);
            Assert.AreEqual(expected, tileIndex);
        }

        [Test]
        public void GetTileIndexFromPosition1D()
        {
            // arrange
            var gridCellCount = new int2(64, 32);
            var gridCellSize = new float2(1.0f, 1.0f);
            var tileCellCount = new int2(10, 10);
            var tileCellSize = new float2(1.0f, 1.0f);
            var position = new float3(14.5f, 0, 20.1f);

            // act
            var tileIndex =
                FlowFieldTileHelper.PositionToTileIndexAtGrid1D(
                    gridCellCount,
                    gridCellSize,
                    tileCellCount,
                    tileCellSize,
                    new float2(position.x, position.z));

            // assert
            var expected = 15;
            Assert.AreEqual(expected, tileIndex);
        }

        [Test]
        public void GetTileIndexFromPosition1DOutGridBoundary()
        {
            // arrange
            var gridCellCount = new int2(64, 32);
            var gridCellSize = new float2(1.0f, 1.0f);
            var tileCellCount = new int2(10, 10);
            var tileCellSize = new float2(1.0f, 1.0f);
            var position = new float3(68.5f, 0, 20.1f);

            // act
            var tileIndex =
                FlowFieldTileHelper.PositionToTileIndexAtGrid1D(
                    gridCellCount,
                    gridCellSize,
                    tileCellCount,
                    tileCellSize,
                    new float2(position.x, position.z));

            // assert
            var expected = -1;
            Assert.AreEqual(expected, tileIndex);
        }

        [Test]
        public void GetPositionToTileAndTileCellIndexAtGridTo2DIndex()
        {
            // arrange
            var gridCellCount = new int2(64, 32);
            var gridCellSize = new float2(1.0f, 1.0f);
            var tileCellCount = new int2(10, 10);
            var tileCellSize = new float2(1.0f, 1.0f);
            var position = new float3(14.5f, 0, 20.1f);

            // act
            var tileIndex =
                FlowFieldTileHelper.PositionToTileAndTileCellIndexAtGridTo2DIndex(
                    gridCellCount,
                    gridCellSize,
                    tileCellCount,
                    tileCellSize,
                    new float2(position.x, position.z));

            // assert
            var expected = new int4(1, 2, 4, 0);
            Assert.AreEqual(expected, tileIndex);
        }

        [Test]
        public void GetPositionToTileAndTileCellIndexAtGridTo1DIndex()
        {
            // arrange
            var gridCellCount = new int2(64, 32);
            var gridCellSize = new float2(1.0f, 1.0f);
            var tileCellCount = new int2(10, 10);
            var tileCellSize = new float2(1.0f, 1.0f);
            var position = new float3(14.5f, 0, 22.1f);

            // act
            var tileIndex =
                FlowFieldTileHelper.PositionToTileAndTileCellIndexAtGridTo1DIndex(
                    gridCellCount,
                    gridCellSize,
                    tileCellCount,
                    tileCellSize,
                    new float2(position.x, position.z));

            // assert
            var expected = new int2(15, 24);
            Assert.AreEqual(expected, tileIndex);
        }
    }
}
