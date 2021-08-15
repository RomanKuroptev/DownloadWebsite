using NUnit.Framework;

namespace ConsoleApp1
{
    [TestFixture]
    public class UrlHelperTest
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("/../../test")]
        [TestCase("../test")]
        [TestCase("/test")]
        public void RemovePrefix_HasPrefix_ReturnsNoPrefix(string test)
        {
            string result = UrlHelper.RemovePrefix(test);
            Assert.AreEqual("/test", result);
        }

        [Test]
        [TestCase("/test.html")]
        public void IsFile_IsFile_ReturnsTrue(string test)
        {
            bool result = UrlHelper.IsFile(test);
            Assert.AreEqual(true, result);
        }

        [Test]
        [TestCase("/test")]
        public void IsFile_IsFolder_Returnsfalse(string test)
        {
            var result = UrlHelper.IsFile(test);
            Assert.AreEqual(false, result);
        }

        [Test]
        [TestCase("test")]
        public void Normalize_StartsWithoutSlash_ReturnsWithSlash(string test)
        {
            var result = UrlHelper.Normalize(test);
            Assert.AreEqual("/test", result);
        }

        [Test]
        [TestCase("htttp://www.test.com")]
        public void IsAbsulteUrl_IsAbsulteUrl_ReturnsTrue(string test)
        {
            bool result = UrlHelper.IsAbsoluteUrl(test);
            Assert.AreEqual(true, result);
        }
    }
}
