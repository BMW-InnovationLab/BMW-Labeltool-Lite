using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robotron.Imagine.Util;
using System.Text.RegularExpressions;

namespace Robotron.Imagine.Tests.Util
{
    [TestClass]
    public class HexColorUtilTests
    {
        #region GetHexColorRegex

        [TestMethod]
        public void GetHexColorRegexRRGGBBTest()
        {
            // Init
            Regex hexColorRegex = HexColorUtil.GetHexColorRegex();
            string hexColor = "#128Fa5";

            // Act
            bool result = hexColorRegex.IsMatch(hexColor);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetHexColorRegexRRGGBBAATest()
        {
            // Init
            Regex hexColorRegex = HexColorUtil.GetHexColorRegex();
            string hexColor = "#128Fa53C";

            // Act
            bool result = hexColorRegex.IsMatch(hexColor);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetHexColorRegexRGBTest()
        {
            // Init
            Regex hexColorRegex = HexColorUtil.GetHexColorRegex();
            string hexColor = "#f5C";

            // Act
            bool result = hexColorRegex.IsMatch(hexColor);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetHexColorRegexRGBATest()
        {
            // Init
            Regex hexColorRegex = HexColorUtil.GetHexColorRegex();
            string hexColor = "#f5C9";

            // Act
            bool result = hexColorRegex.IsMatch(hexColor);

            // Assert
            Assert.IsTrue(result);
        }

        [TestMethod]
        public void GetHexColorRegexLength5Test()
        {
            // Init
            Regex hexColorRegex = HexColorUtil.GetHexColorRegex();
            string hexColor = "#f5C93";

            // Act
            bool result = hexColorRegex.IsMatch(hexColor);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetHexColorRegexLength7Test()
        {
            // Init
            Regex hexColorRegex = HexColorUtil.GetHexColorRegex();
            string hexColor = "#f5C9344";

            // Act
            bool result = hexColorRegex.IsMatch(hexColor);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetHexColorRegexLength9Test()
        {
            // Init
            Regex hexColorRegex = HexColorUtil.GetHexColorRegex();
            string hexColor = "#f5C93dd47";

            // Act
            bool result = hexColorRegex.IsMatch(hexColor);

            // Assert
            Assert.IsFalse(result);
        }

        [TestMethod]
        public void GetHexColorRegexInvalidCharacterTest()
        {
            // Init
            Regex hexColorRegex = HexColorUtil.GetHexColorRegex();
            string hexColor = "#f5C93Td4";

            // Act
            bool result = hexColorRegex.IsMatch(hexColor);

            // Assert
            Assert.IsFalse(result);
        }

        #endregion

        #region GetR

        [TestMethod]
        public void GetRFromRRGGBBAATest()
        {
            // Init
            string hexColor = "#128Fa53C";

            // Act
            byte result = HexColorUtil.GetR(hexColor);

            // Assert
            Assert.AreEqual(18, result);
        }

        [TestMethod]
        public void GetRFromRRGGBBTest()
        {
            // Init
            string hexColor = "#128Fa5";

            // Act
            byte result = HexColorUtil.GetR(hexColor);

            // Assert
            Assert.AreEqual(18, result);
        }

        [TestMethod]
        public void GetRFromRGBATest()
        {
            // Init
            string hexColor = "#465A";

            // Act
            byte result = HexColorUtil.GetR(hexColor);

            // Assert
            Assert.AreEqual(64, result);
        }

        [TestMethod]
        public void GetRFromRGBTest()
        {
            // Init
            string hexColor = "#465";

            // Act
            byte result = HexColorUtil.GetR(hexColor);

            // Assert
            Assert.AreEqual(64, result);
        }

        #endregion

        #region GetG

        [TestMethod]
        public void GetGFromRRGGBBAATest()
        {
            // Init
            string hexColor = "#128Fa53C";

            // Act
            byte result = HexColorUtil.GetG(hexColor);

            // Assert
            Assert.AreEqual(143, result);
        }

        [TestMethod]
        public void GetGFromRRGGBBTest()
        {
            // Init
            string hexColor = "#128Fa5";

            // Act
            byte result = HexColorUtil.GetG(hexColor);

            // Assert
            Assert.AreEqual(143, result);
        }

        [TestMethod]
        public void GetGFromRGBATest()
        {
            // Init
            string hexColor = "#465A";

            // Act
            byte result = HexColorUtil.GetG(hexColor);

            // Assert
            Assert.AreEqual(96, result);
        }

        [TestMethod]
        public void GetGFromRGBTest()
        {
            // Init
            string hexColor = "#465";

            // Act
            byte result = HexColorUtil.GetG(hexColor);

            // Assert
            Assert.AreEqual(96, result);
        }

        #endregion

        #region GetR

        [TestMethod]
        public void GetBFromRRGGBBAATest()
        {
            // Init
            string hexColor = "#128Fa53C";

            // Act
            byte result = HexColorUtil.GetB(hexColor);

            // Assert
            Assert.AreEqual(165, result);
        }

        [TestMethod]
        public void GetBFromRRGGBBTest()
        {
            // Init
            string hexColor = "#128Fa5";

            // Act
            byte result = HexColorUtil.GetB(hexColor);

            // Assert
            Assert.AreEqual(165, result);
        }

        [TestMethod]
        public void GetBFromRGBATest()
        {
            // Init
            string hexColor = "#465A";

            // Act
            byte result = HexColorUtil.GetB(hexColor);

            // Assert
            Assert.AreEqual(80, result);
        }

        [TestMethod]
        public void GetBFromRGBTest()
        {
            // Init
            string hexColor = "#465";

            // Act
            byte result = HexColorUtil.GetB(hexColor);

            // Assert
            Assert.AreEqual(80, result);
        }

        #endregion

        #region GetA

        [TestMethod]
        public void GetAFromRRGGBBAATest()
        {
            // Init
            string hexColor = "#128Fa53C";

            // Act
            byte result = HexColorUtil.GetA(hexColor);

            // Assert
            Assert.AreEqual(60, result);
        }

        [TestMethod]
        public void GetAFromRRGGBBTest()
        {
            // Init
            string hexColor = "#128Fa5";

            // Act
            byte result = HexColorUtil.GetA(hexColor);

            // Assert
            Assert.AreEqual(255, result);
        }

        [TestMethod]
        public void GetAFromRGBATest()
        {
            // Init
            string hexColor = "#465A";

            // Act
            byte result = HexColorUtil.GetA(hexColor);

            // Assert
            Assert.AreEqual(160, result);
        }

        [TestMethod]
        public void GetAFromRGBTest()
        {
            // Init
            string hexColor = "#465";

            // Act
            byte result = HexColorUtil.GetA(hexColor);

            // Assert
            Assert.AreEqual(255, result);
        }

        #endregion
    }
}
