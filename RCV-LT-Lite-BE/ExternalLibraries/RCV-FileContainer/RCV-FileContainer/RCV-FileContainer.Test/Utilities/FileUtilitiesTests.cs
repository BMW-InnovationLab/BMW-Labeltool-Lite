using Microsoft.VisualStudio.TestTools.UnitTesting;
using RCV.FileContainer.Utilities;

namespace RCV_FileContainer.Test.Utilities
{
    [TestClass]
    public class FileUtilitiesTests
    {
        [TestMethod]
        public void GetPathAbsoluteRootTest()
        {
            // init
            FileUtilities fileUtilities = new FileUtilities(@"C:\rcv-data");

            // act 
            string resultPath = fileUtilities.GetPath(new string[] { "C:", "rcv-data", "cars", "images" });

            // assert
            Assert.AreEqual(@"C:\rcv-data\cars\images", resultPath);
        }

        [TestMethod]
        public void GetPathRelativeRootTest()
        {
            // init
            FileUtilities fileUtilities = new FileUtilities(@"/rcv-data");

            // act 
            string resultPath = fileUtilities.GetPath(new string[] { "", "rcv-data", "cars", "images" });

            // assert
            Assert.AreEqual(@"/rcv-data\cars\images", resultPath);
        }

        [TestMethod]
        public void GetPathRelativeRootMultipleTest()
        {
            // init
            FileUtilities fileUtilities = new FileUtilities(@"/rcv/data");

            // act 
            string resultPath = fileUtilities.GetPath(new string[] { "", "rcv", "data", "cars", "images" });

            // assert
            Assert.AreEqual(@"/rcv\data\cars\images", resultPath);
        }
    }
}
