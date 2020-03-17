namespace JoyBrick.Walkio.Game.Environment.Utility.Tests
{
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    
    using Utility = JoyBrick.Walkio.Game.Environment.Utility;
   
    public class TestSuit
    {
        [Test]
        public void RGBValueAll255MapToIndex0()
        {
            // arrange
            var r = 255;
            var g = 255;
            var b = 255;

            // act
            var index = Utility.WorldMapHelper.GetTileTypeIndex(r, g, b);

            // assert
            Assert.IsTrue(index == 0);
        }
        
        [Test]
        public void RGBValueR20MapToIndex1()
        {
            // arrange
            var r = 20;
            var g = 0;
            var b = 0;

            // act
            var index = Utility.WorldMapHelper.GetTileTypeIndex(r, g, b);

            // assert
            Assert.IsTrue(index == 1);
        }
        
        [Test]
        public void RGBValueR240AboveMapToIndex12()
        {
            // arrange
            var r1 = 240;
            var g1 = 0;
            var b1 = 0;

            var r2 = 255;
            var g2 = 0;
            var b2 = 0;

            // act
            var index1 = Utility.WorldMapHelper.GetTileTypeIndex(r1, g1, b1);
            var index2 = Utility.WorldMapHelper.GetTileTypeIndex(r2, g2, b2);

            var expected2 = 12;

            // assert
            Assert.IsTrue(index1 == 12);
            Assert.AreEqual(expected2, index2);
        }
        
        [Test]
        public void RGBValueR240AboveG20MapToIndex13()
        {
            // arrange
            var r1 = 240;
            var g1 = 20;
            var b1 = 0;

            // act
            var index1 = Utility.WorldMapHelper.GetTileTypeIndex(r1, g1, b1);

            var expected1 = 13;

            // assert
            Assert.AreEqual(expected1, index1);
        }
        
        public void RGBValueR240AboveG240AboveB20MapToIndex25()
        {
            // arrange
            var r1 = 240;
            var g1 = 240;
            var b1 = 20;

            // act
            var index1 = Utility.WorldMapHelper.GetTileTypeIndex(r1, g1, b1);

            var expected1 = 25;

            // assert
            Assert.AreEqual(expected1, index1);
        }
        
        [Test]
        public void CheckForAssigningTileIndexAtOrigin()
        {
            // arrange
            var worldWidth = 30;
            var worldHeight = 20;
            var tileWidth = 10;
            var tileHeight = 10;
            var xTileIndex = 0;
            var yTileIndex = 0;
            var xIndex = 0;
            var yIndex = 0;

            // act
            var index =
                Utility.WorldMapHelper.GetTileIndex(
                    worldWidth, worldHeight,
                    tileWidth, tileHeight,
                    xTileIndex, yTileIndex,
                    xIndex, yIndex);
            
            // assert
            Assert.IsTrue(index == 0);
        }

        [Test]
        public void CheckForAssigningTileIndexAtTileX1Y1Origin()
        {
            // arrange
            var worldWidth = 30;
            var worldHeight = 20;
            var tileWidth = 10;
            var tileHeight = 10;
            var xTileIndex = 1;
            var yTileIndex = 1;
            var xIndex = 0;
            var yIndex = 0;

            // act
            var index =
                Utility.WorldMapHelper.GetTileIndex(
                    worldWidth, worldHeight,
                    tileWidth, tileHeight,
                    xTileIndex, yTileIndex,
                    xIndex, yIndex);
            
            // assert
            Assert.IsTrue(index == 310);
        }
    }
}