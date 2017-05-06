using KerbalParser;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace KerbalParserTests
{
    [TestFixture]
    internal class BasicTests
    {
        private KerbalConfig _kerbalConfig;

        [SetUp]
        public void Init()
        {
            const string file = "..\\..\\testdata\\simple.cfg";
            var parser = new Parser();
            _kerbalConfig = parser.ParseConfig(file);
        }

        [TearDown]
        public void CleanUp()
        {
            _kerbalConfig = null;
        }

        [Test]
        public void CreateParser()
        {
            Assert.IsInstanceOf<KerbalConfig>(_kerbalConfig);
        }

        [Test]
        public void GenericParse()
        {
            Assert.IsNotEmpty(_kerbalConfig);

            foreach (var kerbalNode in _kerbalConfig)
            {
                Console.WriteLine(kerbalNode);

                Assert.IsNotNullOrEmpty
                    (kerbalNode.Name);
                //Assert.IsInstanceOf<IDictionary<string, List<string>>>
                //    (kerbalNode.Values);
                Assert.IsInstanceOf<IDictionary<string, string>>(kerbalNode.Values);
                Assert.IsInstanceOf<IList<KerbalNode>>
                    (kerbalNode.Children);
            }
        }

        [Test]
        public void SimpleParse()
        {
            var node = _kerbalConfig.First();

            Assert.AreEqual(24, node.Values.Count);
            Assert.AreEqual(2, node.Children.Count);

            var childOne = node.Children[0];
            var childTwo = node.Children[1];

            Console.Write(childOne);
            Console.WriteLine();

            Assert.AreEqual(3, childOne.Values.Count);

            Console.Write(childTwo);
            Console.WriteLine();

            Assert.AreEqual(2, childTwo.Values.Count);
        }
    }
}
