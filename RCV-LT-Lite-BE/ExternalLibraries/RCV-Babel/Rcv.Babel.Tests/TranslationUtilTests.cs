using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rcv.Babel.Tests.Mock;
using RCV.Babel.Util;

namespace Rcv.Babel.Tests
{
    [TestClass]
    public class TranslationUtilTests
    {
        [TestMethod]
        public void TranslateCultureCountryTest()
        {
            // init
            Car car = new Car();

            // act
            TranslationUtil.Translate(car, "de-DE");

            // assert
            Assert.AreEqual("Lenkung", car.Steering);
            Assert.AreEqual("Getriebe", car.Gearbox);
        }

        [TestMethod]
        public void TranslateCultureTest()
        {
            // init
            Car car = new Car();

            // act
            TranslationUtil.Translate(car, "de");

            // assert
            Assert.AreEqual("Lenkung", car.Steering);
            Assert.AreEqual("Getriebe", car.Gearbox);
        }
    }
}
