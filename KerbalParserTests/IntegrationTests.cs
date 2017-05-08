using System;
using KerbalParser;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace KerbalParserTests
{
    [TestClass]
    public class IntegrationTests
    {
        private KerbalConfig _cfg;
        private Parser _parser;
        private string _testPart;

        [TestInitialize]
        public void Setup()
        {
            _testPart = "..\\..\\testdata\\capsule.cfg";
            _parser = new Parser(false, true);

            _cfg = _parser.ParseConfig(_testPart);
        }

        [TestCleanup]
        public void TearDown()
        {
            _testPart = null;
            _parser = null;
            _cfg = null;
        }

        [TestMethod]
        public void CapsuleTest()
        {
            Assert.IsInstanceOfType(_cfg, typeof(KerbalConfig), $"Not a config, but {_cfg.GetType()}");
            Assert.IsInstanceOfType(_cfg, typeof(IEnumerable<KerbalNode>), $"Parsed {_cfg.GetType()}");
        }

        [TestMethod]
        public void CapsuleHas11Nodes()
        {
            var nodes = _cfg.Nodes.First().Children;
            var nodesCount = nodes.Count;

            Assert.AreEqual(11, nodesCount, $"Expected 11 nodes, but got {nodesCount}");
        }

        [TestMethod]
        public void CapsuleHas34Properties()
        {
            var properties = _cfg.Nodes[0].Values.Count;

            Assert.AreEqual(34, properties, $"Expected 34 porperties but got {properties}");
        }

        [TestMethod]
        public void CapsulePropertiesAreParsedCorrectly()
        {
            var properties = _cfg.First().Values;
            var tech = properties["TechRequired"];
            var cost = properties["cost"];

            Assert.AreEqual("commandModules", tech, $"Part tech is {tech}");
            Assert.AreEqual("4400", cost, $"Part cost is {cost}");
        }

        [TestMethod]
        [ExpectedException(typeof(ParseErrorException))]
        public void WrongPropertiesThrowParseException()
        {
            var properties = _cfg.First().Values;
            var none = properties["none"];
        }

        [TestMethod]
        public void ExceptionReturnsWrongKey()
        {
            string message = string.Empty;
            var properties = _cfg.First().Values;
            try
            {
                var none = properties["none"];
            }
            catch (ParseErrorException ex)
            {
                message = ex.Message;
            }
            StringAssert.Contains(message, "none", "wrong exception message");
        }
    }
}