using System;
using System.Linq;
using KerbalParser;
using NUnit.Framework;

namespace KerbalParserTests
{
    [TestFixture]
    public class ComplexFileTests
    {
        private KerbalConfig _kerbalConfig;

        [SetUp]
        public void Init()
        {
            const string file = "..\\..\\testdata\\complex.cfg";
            var parser = new Parser();
            _kerbalConfig = parser.ParseConfig(file);
        }

        [TearDown]
        public void CleanUp()
        {
            _kerbalConfig = null;
        }

        [Test]
        public void BracketPositions()
        {
            var node = _kerbalConfig.First();

            Console.Write(node);
            Console.WriteLine();

            Assert.AreEqual(4, node.Values.Count);
            Assert.AreEqual(4, node.Children.Count);
            Assert.IsTrue(
                node.Values.ContainsKey("name"),
                "Expected key \"name\" wasn't found."
            );

            var childOne = node.Children[0];
            var childTwo = node.Children[1];
            var childThree = node.Children[2];
            var childFour = node.Children[3];

            Console.Write(childOne);
            Console.WriteLine();

            Assert.AreEqual(3, childOne.Values.Count);
            Assert.IsTrue(
                childOne.Values.ContainsKey("name"),
                "Expected key \"name\" wasn't found."
            );

            Console.Write(childTwo);
            Console.WriteLine();

            Assert.AreEqual(2, childTwo.Values.Count);
            Assert.IsTrue(
                childTwo.Values.ContainsKey("amount"),
                "Expected key \"amount\" wasn't found."
            );

            Console.Write(childThree);
            Console.WriteLine();

            Assert.AreEqual(1, childThree.Values.Count);
            Assert.IsTrue(
                childThree.Values.ContainsKey("name"),
                "Expected key \"name\" wasn't found."
            );

            Console.Write(childFour);
            Console.WriteLine();

            Assert.AreEqual(0, childFour.Values.Count);
        }

        [Test]
        public void EmptyValue()
        {
            var node = _kerbalConfig.First();

            Assert.IsTrue(node.Values.ContainsKey("author"));
            Assert.IsTrue(node.Values["author"].First() == "");
        }
    }
}