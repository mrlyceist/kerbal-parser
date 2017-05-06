using System;
using KerbalParser;
using NUnit.Framework;

namespace KerbalParserTests
{
    [TestFixture]
    public class MultiTreeParserTests
    {
        private KerbalConfig _kerbalConfig;

        [SetUp]
        public void Init()
        {
            const string file = "..\\..\\testdata\\multi.cfg";
            var parser = new Parser();
            _kerbalConfig = parser.ParseConfig(file);
        }

        [TearDown]
        public void CleanUp()
        {
            _kerbalConfig = null;
        }

        [Test]
        public void BasicMultiTreeParse()
        {
            foreach (var tree in _kerbalConfig)
            {
                Console.WriteLine(tree);
                Console.WriteLine();
            }
            Assert.AreEqual(2, _kerbalConfig.Count);
        }

        [Test]
        public void ValidMultiTreeParse()
        {
            foreach (var tree in _kerbalConfig)
            {
                Console.WriteLine(tree);
                Console.WriteLine();
            }

            var treeTwo = _kerbalConfig[1];

            Assert.AreEqual(3, treeTwo.Values.Count);
            Assert.AreEqual(2, treeTwo.Children.Count);

            var childOne = treeTwo.Children[0];
            var childTwo = treeTwo.Children[1];

            Console.WriteLine(childOne);
            Console.WriteLine();

            Assert.AreEqual(1, childOne.Values.Count);

            Console.WriteLine(childTwo);

            Assert.AreEqual(2, childTwo.Values.Count);
        }
    }
}