using Microsoft.VisualStudio.TestTools.UnitTesting;
using RCV.Babel.Util;

namespace Rcv.Babel.Tests
{
    [TestClass]
    public class LanguageUtilTests
    {
        [TestMethod]
        public void GetCultureFromAcceptLanguageTest()
        {
            // init
            string header = "fr-CH, fr;q=0.9, en;q=0.8, de;q=0.7, *;q=0.5";

            // act
            string culture = LanguageUtil.GetCultureFromAcceptLanguage(header);

            // assert
            Assert.AreEqual("fr-CH", culture);
        }

        [TestMethod]
        public void GetCultureFromAcceptLanguageCountryWithQTest()
        {
            // init
            string header = "de;q=0.7";

            // act
            string culture = LanguageUtil.GetCultureFromAcceptLanguage(header);

            // assert
            Assert.AreEqual("de", culture);
        }

        [TestMethod]
        public void GetCultureFromAcceptLanguageCountryAndRegionTest()
        {
            // init
            string header = "de-AT";

            // act
            string culture = LanguageUtil.GetCultureFromAcceptLanguage(header);

            // assert
            Assert.AreEqual("de-AT", culture);
        }

        [TestMethod]
        public void GetCultureFromAcceptLanguageCountryTest()
        {
            // init
            string header = "de";

            // act
            string culture = LanguageUtil.GetCultureFromAcceptLanguage(header);

            // assert
            Assert.AreEqual("de", culture);
        }
    }
}
