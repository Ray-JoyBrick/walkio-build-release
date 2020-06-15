namespace JoyBrick.Walkio.Game.Common.Utility.Tests
{
    using NUnit.Framework;
    using UnityEngine;

    public class Color32ExtensionTest
    {
        [Test]
        public void PureBlackValueIs0()
        {
            // arrange
            var c = new Color32(0, 0, 0, 0);

            // act
            var expected = 0;
            var actual = c.ToInt();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void PureBlueValueIs255()
        {
            // arrange
            var c = new Color32(0, 0, 255, 0);

            // act
            var expected = 255;
            var actual = c.ToInt();

            // assert
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void Value65535IsColorR0G255B255A0()
        {
            // arrange
            var v = 65535;

            // act
            var expected = new Color32(0, 255, 255, 0);
            var actual = (new Color32()).FromInt(v);

            // assert
            Assert.AreEqual(expected, actual);
        }
    }
}
