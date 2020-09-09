namespace JoyBrick.Walkio.Game.Move.FlowField.Utility.EditorPart.Tests
{
    using NUnit.Framework;
    using Unity.Mathematics;
    using UnityEngine;
    using UnityEngine.TestTools;

    using GameMoveFlowFieldUtility = JoyBrick.Walkio.Game.Move.FlowField.Utility;

    public class GridWorldHelperTest
    {
        [Test]
        public void GetGridCellIndexFromPosition()
        {
            // arrange
            var gridCellCount = new int2(10, 10);
            var gridCellSize = new float2(1.0f, 1.0f);
            var position01 = new float3(4.0f, 0, 1.9f);
            var position02 = new float3(0.1f, 0, 0.01f);

            // act
            var gridCellIndex01 =
                GridWorldHelper.PositionToGridCellIndex2D(
                    gridCellCount,
                    gridCellSize,
                    new float2(position01.x, position01.z));
            var gridCellIndex02 =
                GridWorldHelper.PositionToGridCellIndex2D(
                    gridCellCount,
                    gridCellSize,
                    new float2(position02.x, position02.z));

            // assert
            var expected01 = new int2(4, 1);
            var expected02 = new int2(0, 0);
            Assert.AreEqual(expected01, gridCellIndex01);
            Assert.AreEqual(expected02, gridCellIndex02);
        }
    }
}
