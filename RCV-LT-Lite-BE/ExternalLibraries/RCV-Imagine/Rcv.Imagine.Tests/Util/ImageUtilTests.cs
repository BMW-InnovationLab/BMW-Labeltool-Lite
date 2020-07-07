using Microsoft.VisualStudio.TestTools.UnitTesting;
using Robotron.Imagine.Model;
using Robotron.Imagine.Util;
using System.IO;

namespace Robotron.Imagine.Tests.Util
{
    [TestClass]
    public class ImageUtilTests
    {
        [TestMethod]
        public void GetDimensionFromPngImageTest()
        {
            Rect imageDimensions = null;
            using (System.IO.Stream imageStream = File.OpenRead(@"Testdata\schraube.png"))
            {
                imageDimensions = ImageUtil.GetDimensionFromImage(imageStream);
            }

            Assert.IsNotNull(imageDimensions);
            Assert.AreEqual(0, imageDimensions.X);
            Assert.AreEqual(0, imageDimensions.Y);
            Assert.AreEqual(200, imageDimensions.Width);
            Assert.AreEqual(200, imageDimensions.Height);
        }

        [TestMethod]
        public void GetDimensionFromJpgImageTest()
        {
            Rect imageDimensions = null;

            using (System.IO.Stream imageStream = File.OpenRead(@"Testdata\2008_003743.jpg"))
            {
                imageDimensions = ImageUtil.GetDimensionFromImage(imageStream);
            }

            Assert.IsNotNull(imageDimensions);
            Assert.AreEqual(0, imageDimensions.X);
            Assert.AreEqual(0, imageDimensions.Y);
            Assert.AreEqual(500, imageDimensions.Width);
            Assert.AreEqual(375, imageDimensions.Height);
        }

        [TestMethod]
        public void GetDimensionOutFromImageTest()
        {
            int width = 0;
            int height = 0;
            using (System.IO.Stream imageStream = File.OpenRead(@"Testdata\2008_003743.jpg"))
            {
                ImageUtil.GetDimensionFromImage(imageStream, out width, out height);
            }

            Assert.AreEqual(500, width);
            Assert.AreEqual(375, height);
        }

        [TestMethod]
        public void GetDimensionFromImageByteArrayTest()
        {
            Rect imageDimensions = null;
            using (System.IO.Stream imageStream = File.OpenRead(@"Testdata\2008_003743.jpg"))
            using (System.IO.MemoryStream memoryStream = new MemoryStream())
            {
                imageStream.CopyTo(memoryStream);

                imageDimensions = ImageUtil.GetDimensionFromImage(memoryStream.ToArray());
            }

            Assert.IsNotNull(imageDimensions);
            Assert.AreEqual(0, imageDimensions.X);
            Assert.AreEqual(0, imageDimensions.Y);
            Assert.AreEqual(500, imageDimensions.Width);
            Assert.AreEqual(375, imageDimensions.Height);
        }

        [TestMethod]
        public void GetDimensionOutFromImageByteArrayTest()
        {
            int width = 0;
            int height = 0;
            using (System.IO.Stream imageStream = File.OpenRead(@"Testdata\2008_003743.jpg"))
            using (System.IO.MemoryStream memoryStream = new MemoryStream())
            {
                imageStream.CopyTo(memoryStream);

                ImageUtil.GetDimensionFromImage(memoryStream.ToArray(), out width, out height);
            }

            Assert.AreEqual(500, width);
            Assert.AreEqual(375, height);
        }

        [TestMethod]
        public void TryOpenImageTest()
        {
            using (System.IO.Stream imageStream = File.OpenRead(@"Testdata\2008_003743.jpg"))
            {
                ImageUtil.TryOpenImage(imageStream);

                Assert.AreEqual(0L, imageStream.Position);
            }
        }

        [TestMethod]
        public void TryOpenImageByteArrayTest()
        {
            using (System.IO.Stream imageStream = File.OpenRead(@"Testdata\2008_003743.jpg"))
            using (System.IO.MemoryStream memoryStream = new MemoryStream())
            {
                imageStream.CopyTo(memoryStream);

                ImageUtil.TryOpenImage(memoryStream.ToArray());
            }
        }

        [TestMethod]
        public void TryOpenCorruptedImageTest()
        {
            try
            {
                using (System.IO.Stream imageStream = File.OpenRead(@"Testdata\corruptImage.png"))
                {
                    ImageUtil.TryOpenImage(imageStream);
                    Assert.Fail();
                }
            }
            catch (System.Exception)
            {

            }
        }
    }
}
