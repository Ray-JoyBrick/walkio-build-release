namespace JoyBrick.Walkio.Game.Move.FlowField.Utility.EditorPart.Tests
{
    using NUnit.Framework;
    using Unity.Collections;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.TestTools;

    using GameMoveFlowFieldUtility = JoyBrick.Walkio.Game.Move.FlowField.Utility; 

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
                GameMoveFlowFieldUtility.FlowFieldTileHelper.GetTileCountFromGrid2D(
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
                GameMoveFlowFieldUtility.FlowFieldTileHelper.GetTileCountFromGrid1D(
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
                GameMoveFlowFieldUtility.FlowFieldTileHelper.PositionToTileIndexAtGrid2D(
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
                GameMoveFlowFieldUtility.FlowFieldTileHelper.PositionToTileIndexAtGrid1D(
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
                GameMoveFlowFieldUtility.FlowFieldTileHelper.PositionToTileIndexAtGrid1D(
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
                GameMoveFlowFieldUtility.FlowFieldTileHelper.PositionToTileAndTileCellIndexAtGridTo2DIndex(
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
                GameMoveFlowFieldUtility.FlowFieldTileHelper.PositionToTileAndTileCellIndexAtGridTo1DIndex(
                    gridCellCount,
                    gridCellSize,
                    tileCellCount,
                    tileCellSize,
                    new float2(position.x, position.z));
        
            // assert
            var expected = new int2(15, 24);
            Assert.AreEqual(expected, tileIndex);
        }
        
        [Test]
        public void GetGridCellIndexToTileAndTileCellIndexAtGridTo1DIndex()
        {
            // arrange
            var gridCellCount = new int2(64, 32);
            var gridCellSize = new float2(1.0f, 1.0f);
            var tileCellCount = new int2(10, 10);
            var tileCellSize = new float2(1.0f, 1.0f);

            var tileIndex1 = new int2(0, 0);
            var tileCellIndex1 = new int2(0, 0);

            // var tileIndex2 = new int2(1, 0);
            // var tileCellIndex2 = new int2(2, 2);
        
            // act
            var gridCellIndex1 =
                GameMoveFlowFieldUtility.FlowFieldTileHelper.FromTileAndTileCellIndexToGridCellIndex2D(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize,
                    tileIndex1, tileCellIndex1);

            // var gridCellIndex2 =
            //     GameEnvironmentUtility.FlowFieldTile.FromTileAndTileCellIndexToGridCellIndex2D(
            //         gridCellCount, gridCellSize,
            //         tileCellCount, tileCellSize,
            //         tileIndex2, tileCellIndex2);

            // assert
            var expected1 = new int2(0, 0);
            var expected2 = new int2(0, 0);
            Assert.AreEqual(expected1, gridCellIndex1);
            // Assert.AreEqual(expected2, gridCellIndex2);
        }
        
        [Test]
        public void GetTileIndexFromGridCellIndex1D()
        {
            // arrange
            var gridCellCount = new int2(64, 32);
            var gridCellSize = new float2(1.0f, 1.0f);
            var tileCellCount = new int2(10, 10);
            var tileCellSize = new float2(1.0f, 1.0f);

            var gridCellIndex1 = new int2(0, 0);
            
            var gridCellIndex2 = new int2(11, 11);

            // act
            var tileIndex1 =
                GameMoveFlowFieldUtility.FlowFieldTileHelper.FromGridCellIndexToTileIndex2D(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize,
                    gridCellIndex1);

            var tileIndex2 =
                GameMoveFlowFieldUtility.FlowFieldTileHelper.FromGridCellIndexToTileIndex2D(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize,
                    gridCellIndex2);

            // assert
            var expected1 = new int2(0, 0);
            var expected2 = new int2(1, 1);
            Assert.AreEqual(expected1, tileIndex1);
            Assert.AreEqual(expected2, tileIndex2);
        }

        [Test]
        public void GetIntegrationCostFor04_04TileCase00_00()
        {
            // arrange
            var gridCellCount = new int2(64, 32);
            var gridCellSize = new float2(1.0f, 1.0f);
            var tileCellCount = new int2(4, 4);
            var tileCellSize = new float2(1.0f, 1.0f);
            
            var goalTileCellIndex = new int2(0, 0);
            
            var baseCosts = new NativeArray<int>(tileCellCount.x * tileCellCount.y, Allocator.Temp);

            baseCosts[ 0] = 0;
            baseCosts[ 1] = 0;
            baseCosts[ 2] = 0;
            baseCosts[ 3] = 0;
            baseCosts[ 4] = 0;
            baseCosts[ 5] = 0;
            baseCosts[ 6] = 0;
            baseCosts[ 7] = 0;
            baseCosts[ 8] = 0;
            baseCosts[ 9] = 0;
            baseCosts[10] = 0;
            baseCosts[11] = 0;
            baseCosts[12] = 0;
            baseCosts[13] = 0;
            baseCosts[14] = 0;
            baseCosts[15] = 0;
            
            // act
            var integrationCosts =
                GameEnvironmentUtility.FlowFieldTileHelper.GetIntegrationCostForTile(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize,
                    goalTileCellIndex, baseCosts);

            for (var i = 0; i < integrationCosts.Length; ++i)
            {
                Debug.Log($"integrationCosts[{i}]: {integrationCosts[i]}");
            }
            
            // assert
            Assert.AreEqual(0, integrationCosts[ 0]);
            Assert.AreEqual(10, integrationCosts[ 1]);
            Assert.AreEqual(20, integrationCosts[ 2]);
            Assert.AreEqual(30, integrationCosts[ 3]);
            Assert.AreEqual(10, integrationCosts[ 4]);
            Assert.AreEqual(14, integrationCosts[ 5]);
            Assert.AreEqual(24, integrationCosts[ 6]);
            Assert.AreEqual(34, integrationCosts[ 7]);
            Assert.AreEqual(20, integrationCosts[ 8]);
            Assert.AreEqual(24, integrationCosts[ 9]);
            Assert.AreEqual(28, integrationCosts[10]);
            Assert.AreEqual(38, integrationCosts[11]);
            Assert.AreEqual(30, integrationCosts[12]);
            Assert.AreEqual(34, integrationCosts[13]);
            Assert.AreEqual(38, integrationCosts[14]);
            Assert.AreEqual(42, integrationCosts[15]);

            if (integrationCosts.IsCreated)
            {
                integrationCosts.Dispose();
            }
        }
        
        [Test]
        public void GetIntegrationCostFor04_04TileCase02_02()
        {
            // arrange
            var gridCellCount = new int2(64, 32);
            var gridCellSize = new float2(1.0f, 1.0f);
            var tileCellCount = new int2(4, 4);
            var tileCellSize = new float2(1.0f, 1.0f);
            
            var goalTileCellIndex = new int2(2, 2);
            
            var baseCosts = new NativeArray<int>(tileCellCount.x * tileCellCount.y, Allocator.Temp);

            baseCosts[ 0] = 0;
            baseCosts[ 1] = 0;
            baseCosts[ 2] = 0;
            baseCosts[ 3] = 0;
            baseCosts[ 4] = 0;
            baseCosts[ 5] = 0;
            baseCosts[ 6] = 0;
            baseCosts[ 7] = 0;
            baseCosts[ 8] = 0;
            baseCosts[ 9] = 0;
            baseCosts[10] = 0;
            baseCosts[11] = 0;
            baseCosts[12] = 0;
            baseCosts[13] = 0;
            baseCosts[14] = 0;
            baseCosts[15] = 0;
            
            // act
            var integrationCosts =
                GameMoveFlowFieldUtility.FlowFieldTileHelper.GetIntegrationCostForTile(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize,
                    goalTileCellIndex, baseCosts);

            for (var i = 0; i < integrationCosts.Length; ++i)
            {
                Debug.Log($"integrationCosts[{i}]: {integrationCosts[i]}");
            }
            
            // assert
            Assert.AreEqual(28, integrationCosts[ 0]);
            Assert.AreEqual(24, integrationCosts[ 1]);
            Assert.AreEqual(20, integrationCosts[ 2]);
            Assert.AreEqual(24, integrationCosts[ 3]);
            Assert.AreEqual(24, integrationCosts[ 4]);
            Assert.AreEqual(14, integrationCosts[ 5]);
            Assert.AreEqual(10, integrationCosts[ 6]);
            Assert.AreEqual(14, integrationCosts[ 7]);
            Assert.AreEqual(20, integrationCosts[ 8]);
            Assert.AreEqual(10, integrationCosts[ 9]);
            Assert.AreEqual( 0, integrationCosts[10]);
            Assert.AreEqual(10, integrationCosts[11]);
            Assert.AreEqual(24, integrationCosts[12]);
            Assert.AreEqual(14, integrationCosts[13]);
            Assert.AreEqual(10, integrationCosts[14]);
            Assert.AreEqual(14, integrationCosts[15]);

            if (integrationCosts.IsCreated)
            {
                integrationCosts.Dispose();
            }
        }
        
        [Test]
        public void GetDirectionFor04_04TileCase02_02()
        {
            // arrange
            var gridCellCount = new int2(64, 32);
            var gridCellSize = new float2(1.0f, 1.0f);
            var tileCellCount = new int2(4, 4);
            var tileCellSize = new float2(1.0f, 1.0f);
            
            var goalTileCellIndex = new int2(2, 2);
            
            var baseCosts = new NativeArray<int>(tileCellCount.x * tileCellCount.y, Allocator.Temp);

            baseCosts[ 0] = 0;
            baseCosts[ 1] = 0;
            baseCosts[ 2] = 0;
            baseCosts[ 3] = 0;
            baseCosts[ 4] = 0;
            baseCosts[ 5] = 0;
            baseCosts[ 6] = 0;
            baseCosts[ 7] = 0;
            baseCosts[ 8] = 0;
            baseCosts[ 9] = 0;
            baseCosts[10] = 0;
            baseCosts[11] = 0;
            baseCosts[12] = 0;
            baseCosts[13] = 0;
            baseCosts[14] = 0;
            baseCosts[15] = 0;
            
            // act
            var integrationCosts =
                GameMoveFlowFieldUtility.FlowFieldTileHelper.GetDirectionForTile(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize,
                    goalTileCellIndex, baseCosts);

            for (var i = 0; i < integrationCosts.Length; ++i)
            {
                Debug.Log($"integrationCosts[{i}]: {integrationCosts[i]}");
            }
            
            // assert
            Assert.AreEqual(2, integrationCosts[ 0]);
            Assert.AreEqual(2, integrationCosts[ 1]);
            Assert.AreEqual(1, integrationCosts[ 2]);
            Assert.AreEqual(0, integrationCosts[ 3]);
            Assert.AreEqual(2, integrationCosts[ 4]);
            Assert.AreEqual(2, integrationCosts[ 5]);
            Assert.AreEqual(1, integrationCosts[ 6]);
            Assert.AreEqual(0, integrationCosts[ 7]);
            Assert.AreEqual(5, integrationCosts[ 8]);
            Assert.AreEqual(5, integrationCosts[ 9]);
            Assert.AreEqual(4, integrationCosts[10]);
            Assert.AreEqual(3, integrationCosts[11]);
            Assert.AreEqual(8, integrationCosts[12]);
            Assert.AreEqual(8, integrationCosts[13]);
            Assert.AreEqual(7, integrationCosts[14]);
            Assert.AreEqual(6, integrationCosts[15]);

            if (integrationCosts.IsCreated)
            {
                integrationCosts.Dispose();
            }
        }
        
        [Test]
        public void GetTileIndexPairOnPath()
        {
            // arrange
            var gridCellCount = new int2(12, 12);
            var gridCellSize = new float2(1.0f, 1.0f);
            var tileCellCount = new int2(4, 4);
            var tileCellSize = new float2(1.0f, 1.0f);
            
            var pathPoints = new NativeArray<float2>(22, Allocator.Temp);
            pathPoints[ 0] = new float2(1.2f,  5.2f);
            pathPoints[ 1] = new float2(1.5f,  5.8f);
            pathPoints[ 2] = new float2(2.2f,  6.2f);
            pathPoints[ 3] = new float2(2.7f,  6.5f);
            pathPoints[ 4] = new float2(3.5f,  6.5f);
            pathPoints[ 5] = new float2(4.2f,  6.6f);
            pathPoints[ 6] = new float2(4.4f,  6.9f);
            pathPoints[ 7] = new float2(4.5f,  7.5f);
            pathPoints[ 8] = new float2(4.6f,  7.9f);
            pathPoints[ 9] = new float2(4.7f,  8.1f);
            pathPoints[10] = new float2(4.7f,  9.5f);
            pathPoints[11] = new float2(4.8f, 10.2f);
            pathPoints[12] = new float2(5.1f, 10.5f);
            pathPoints[13] = new float2(6.2f, 10.2f);
            pathPoints[14] = new float2(6.3f,  9.2f);
            pathPoints[15] = new float2(6.3f,  8.6f);
            pathPoints[16] = new float2(6.2f,  7.5f);
            pathPoints[17] = new float2(6.3f,  6.5f);
            pathPoints[18] = new float2(6.8f,  5.1f);
            pathPoints[19] = new float2(6.2f,  4.6f);
            pathPoints[20] = new float2(6.3f,  3.7f);
            pathPoints[21] = new float2(5.5f,  2.5f);

            // act
            var tileIndexPairs =
                GameMoveFlowFieldUtility.FlowFieldTileHelper.GetTileIndexPairOnPath(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize,
                    pathPoints);

            for (var i = 0; i < tileIndexPairs.Length; ++i)
            {
                Debug.Log($"tileIndexPairs[{i}]: {tileIndexPairs[i]}");
            }
            
            // assert
            Assert.AreEqual(new int2(3, 4), tileIndexPairs[ 0]);
            Assert.AreEqual(new int2(4, 7), tileIndexPairs[ 1]);
            Assert.AreEqual(new int2(7, 4), tileIndexPairs[ 2]);
            Assert.AreEqual(new int2(4, 1), tileIndexPairs[ 3]);


            if (pathPoints.IsCreated)
            {
                pathPoints.Dispose();
            }
        }
        
        [Test]
        public void GetTilePairInfoOnPath()
        {
            // arrange
            var gridCellCount = new int2(12, 12);
            var gridCellSize = new float2(1.0f, 1.0f);
            var tileCellCount = new int2(4, 4);
            var tileCellSize = new float2(1.0f, 1.0f);
            
            var pathPoints = new NativeArray<float2>(22, Allocator.Temp);
            pathPoints[ 0] = new float2(1.2f,  5.2f);
            pathPoints[ 1] = new float2(1.5f,  5.8f);
            pathPoints[ 2] = new float2(2.2f,  6.2f);
            pathPoints[ 3] = new float2(2.7f,  6.5f);
            pathPoints[ 4] = new float2(3.5f,  6.5f);
            pathPoints[ 5] = new float2(4.2f,  6.6f);
            pathPoints[ 6] = new float2(4.4f,  6.9f);
            pathPoints[ 7] = new float2(4.5f,  7.5f);
            pathPoints[ 8] = new float2(4.6f,  7.9f);
            pathPoints[ 9] = new float2(4.7f,  8.1f);
            pathPoints[10] = new float2(4.7f,  9.5f);
            pathPoints[11] = new float2(4.8f, 10.2f);
            pathPoints[12] = new float2(5.1f, 10.5f);
            pathPoints[13] = new float2(6.2f, 10.2f);
            pathPoints[14] = new float2(6.3f,  9.2f);
            pathPoints[15] = new float2(6.3f,  8.6f);
            pathPoints[16] = new float2(6.2f,  7.5f);
            pathPoints[17] = new float2(6.3f,  6.5f);
            pathPoints[18] = new float2(6.8f,  5.1f);
            pathPoints[19] = new float2(6.2f,  4.6f);
            pathPoints[20] = new float2(6.3f,  3.7f);
            pathPoints[21] = new float2(5.5f,  2.5f);

            // act
            var tilePairInfos =
                GameMoveFlowFieldUtility.FlowFieldTileHelper.GetTilePairInfoOnPath(
                    gridCellCount, gridCellSize,
                    tileCellCount, tileCellSize,
                    pathPoints);

            for (var i = 0; i < tilePairInfos.Length; ++i)
            {
                Debug.Log($"tileIndexPairs[{i}]: {tilePairInfos[i]}");
            }
            
            // assert
            Assert.AreEqual(new FlowFieldTileHelper.TilePairInfo
            {
                OutTileIndex = 3,
                InTileIndex = 4,
                OutTileCellIndex = 11,
                InTileCellIndex = 8
            }, tilePairInfos[ 0]);
            Assert.AreEqual(new FlowFieldTileHelper.TilePairInfo
            {
                OutTileIndex = 4,
                InTileIndex = 7,
                OutTileCellIndex = 12,
                InTileCellIndex = 0
            }, tilePairInfos[ 1]);
            Assert.AreEqual(new FlowFieldTileHelper.TilePairInfo
            {
                OutTileIndex = 7,
                InTileIndex = 4,
                OutTileCellIndex = 2,
                InTileCellIndex = 14
            }, tilePairInfos[ 2]);
            Assert.AreEqual(new FlowFieldTileHelper.TilePairInfo
            {
                OutTileIndex = 4,
                InTileIndex = 1,
                OutTileCellIndex = 2,
                InTileCellIndex = 14
            }, tilePairInfos[ 3]);


            if (pathPoints.IsCreated)
            {
                pathPoints.Dispose();
            }
        }

    }
}
