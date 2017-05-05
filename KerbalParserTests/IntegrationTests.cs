using KerbalParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;

namespace KerbalParserTests
{
    [TestClass]
    public class IntegrationTests
    {
        [TestMethod]
        public void CapsuleTest()
        {
            var testPart = "..\\..\\testdata\\capsule.cfg";
            var parser = new Parser(false, true);

            var cfg = parser.ParseConfig(testPart);

            Assert.IsInstanceOfType(cfg, typeof(IEnumerable<KerbalConfig>), $"Parsed {cfg.GetType()}");
        }
    }
}