using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Rcv.HttpUtils.Tests
{
    [TestClass]
    public class UriUtilTests
    {
        [TestMethod]
        public void GetUriNoSlashStringTest()
        {
            // act
            Uri result = UriUtil.GetUri("http://foo.bar", "path", "parameter");

            // assert
            Assert.AreEqual(result.ToString(), "http://foo.bar/path/parameter");
        }

        [TestMethod]
        public void GetUriNoSlashUriTest()
        {
            // act
            Uri result = UriUtil.GetUri(new Uri("http://foo.bar"), "path", "parameter");

            // assert
            Assert.AreEqual(result.ToString(), "http://foo.bar/path/parameter");
        }

        [TestMethod]
        public void GetUriWithSlashStringTest()
        {
            // act
            Uri result = UriUtil.GetUri("http://foo.bar/", "path", "parameter");

            // assert
            Assert.AreEqual(result.ToString(), "http://foo.bar/path/parameter");
        }

        [TestMethod]
        public void GetUriWithSlashUriTest()
        {
            // act
            Uri result = UriUtil.GetUri("http://foo.bar/", "path", "parameter");

            // assert
            Assert.AreEqual(result.ToString(), "http://foo.bar/path/parameter");
        }

        [TestMethod()]
        public void GetUriStringTest()
        {
            // act
            Uri result = UriUtil.GetUri("http://localhost:8080", "/test/", "configuration");

            // assert
            Assert.AreEqual("http://localhost:8080/test/configuration", result.AbsoluteUri);
        }

        [TestMethod()]
        public void GetUriUriTest()
        {
            // act
            Uri result = UriUtil.GetUri(new Uri("http://localhost:8080"), "/test/", "configuration");

            // assert
            Assert.AreEqual("http://localhost:8080/test/configuration", result.AbsoluteUri);
        }

        [TestMethod()]
        public void GetUriMoreSlashesStringTest()
        {
            // act
            Uri result = UriUtil.GetUri("http://localhost:8080/", "/test/", "/configuration");

            // assert
            Assert.AreEqual("http://localhost:8080/test/configuration", result.AbsoluteUri);
        }

        [TestMethod()]
        public void GetUriMoreSlashesUriTest()
        {
            // act
            Uri result = UriUtil.GetUri(new Uri("http://localhost:8080/"), "/test/", "/configuration");

            // assert
            Assert.AreEqual("http://localhost:8080/test/configuration", result.AbsoluteUri);
        }

        [TestMethod()]
        public void GetUriSingleParamStringTest()
        {
            // act
            Uri result = UriUtil.GetUri("http://localhost:8080", "/test/", "configuration", "?p1=1");

            // assert
            Assert.AreEqual("http://localhost:8080/test/configuration?p1=1", result.AbsoluteUri);
        }

        [TestMethod()]
        public void GetUriSingleParamUriTest()
        {
            // act
            Uri result = UriUtil.GetUri(new Uri("http://localhost:8080"), "/test/", "configuration", "?p1=1");

            // assert
            Assert.AreEqual("http://localhost:8080/test/configuration?p1=1", result.AbsoluteUri);
        }

        [TestMethod()]
        public void GetUriMultipleParamStringTest()
        {
            // act
            Uri result = UriUtil.GetUri("http://localhost:8080", "/test/", "configuration", "?p1=1", ";p2=2");

            // assert
            Assert.AreEqual("http://localhost:8080/test/configuration?p1=1;p2=2", result.AbsoluteUri);
        }

        [TestMethod()]
        public void GetUriMultipleParamUriTest()
        {
            // act
            Uri result = UriUtil.GetUri(new Uri("http://localhost:8080"), "/test/", "configuration", "?p1=1", ";p2=2");

            // assert
            Assert.AreEqual("http://localhost:8080/test/configuration?p1=1;p2=2", result.AbsoluteUri);
        }

        [TestMethod()]
        public void GetUriSingleEmptyPathsStringTest()
        {
            // act
            Uri result = UriUtil.GetUri("http://localhost:8080", "");

            // assert
            Assert.AreEqual("http://localhost:8080/", result.AbsoluteUri);
        }

        [TestMethod()]
        public void GetUriSingleEmptyPathsUriTest()
        {
            // act
            Uri result = UriUtil.GetUri(new Uri("http://localhost:8080"), "");

            // assert
            Assert.AreEqual("http://localhost:8080/", result.AbsoluteUri);
        }


        [TestMethod()]
        public void GetUriMultipleEmptyPathsStringTest()
        {
            // act
            Uri result = UriUtil.GetUri("http://localhost:8080", "", "test", "", "", "configuration");

            // assert
            Assert.AreEqual("http://localhost:8080/test/configuration", result.AbsoluteUri);
        }

        [TestMethod()]
        public void GetUriMultipleEmptyPathsUriTest()
        {
            // act
            Uri result = UriUtil.GetUri(new Uri("http://localhost:8080"), "", "test", "", "", "configuration");

            // assert
            Assert.AreEqual("http://localhost:8080/test/configuration", result.AbsoluteUri);
        }

        [TestMethod()]
        public void GetUriWithBaseUriStringTest()
        {
            // act
            Uri result = UriUtil.GetUri("http://rcvgateway/api/objectdetection", "v1", "models", "bauteil2_riss:predict");

            // assert
            Assert.AreEqual("http://rcvgateway/api/objectdetection/v1/models/bauteil2_riss:predict", result.AbsoluteUri);
        }

        [TestMethod()]
        public void GetUriWithBaseUriUriTest()
        {
            // act
            Uri result = UriUtil.GetUri(new Uri("http://rcvgateway/api/objectdetection"), "v1", "models", "bauteil2_riss:predict");

            // assert
            Assert.AreEqual("http://rcvgateway/api/objectdetection/v1/models/bauteil2_riss:predict", result.AbsoluteUri);
        }
    }
}
