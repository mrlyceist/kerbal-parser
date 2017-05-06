using KerbalParser;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KerbalParserTests
{
    [TestFixture]
    public class AdvancedParseTests
    {
        [Test]
        public void DuplicateValues()
        {
            const string file = "..\\..\\testdata\\fail\\duplicatevalues.cfg";
            var parser = new Parser();
            var kc = parser.ParseConfig(file);

            var tree = kc.First();

            Console.WriteLine(tree);

            //Assert.AreEqual(4, tree.Values.Count);
            Assert.IsTrue(tree.Values.ContainsKey("name"));
            Assert.IsTrue(tree.Values.ContainsKey("module"));
            Assert.IsTrue(tree.Values.ContainsKey("author"));
            Assert.IsTrue(tree.Values.ContainsKey("description"));

            // WHY DO WE EVER NEED THIS?!
            //Assert.AreEqual(3, tree.Values["name"].Count);
            //Assert.AreEqual(2, tree.Values["author"].Count);
            //Assert.IsTrue(tree.Values["name"].First() == "batteryBankMini");
            //Assert.IsTrue(tree.Values["name"][1] == "batteryBankMini2");
            //Assert.IsTrue(tree.Values["name"][2] == "spice");
            //Assert.IsTrue(tree.Values["author"].First() == "Squad");
            //Assert.IsTrue(tree.Values["author"][1] == "Bob");
        }

        [Test]
        public void HeadLessConfig()
        {
            const string file = "..\\..\\testdata\\headless.cfg";
            var parser = new Parser();
            var kc = parser.ParseConfig(file);

            var tree = kc.First();

            Console.WriteLine(tree);

            Assert.AreEqual("HEADLESS", tree.Name);
            Assert.AreEqual(7, tree.Values.Count);
            Assert.IsTrue(tree.Values.ContainsKey("name"));
            Assert.IsTrue(tree.Values.ContainsKey("module"));
            Assert.IsTrue(tree.Values.ContainsKey("author"));
            Assert.IsTrue(tree.Values.ContainsKey("mesh"));
            Assert.IsTrue(tree.Values.ContainsKey("rescaleFactor"));
            Assert.IsTrue(tree.Values.ContainsKey("middleprop"));
            Assert.IsTrue(tree.Values.ContainsKey("lastprop"));

            Assert.AreEqual(2, tree.Children.Count);
            Assert.AreEqual(3, tree.Children[0].Values.Count);
            Assert.AreEqual(2, tree.Children[1].Values.Count);
        }

        [Test]
        public void HeadLessTrailConfig()
        {
            const string file = "..\\..\\testdata\\headlesstrail.cfg";
            var parser = new Parser();
            var kc = parser.ParseConfig(file);

            var tree = kc.First();

            Console.WriteLine(tree);

            Assert.AreEqual("HEADLESSTRAIL", tree.Name);
            Assert.AreEqual(2, tree.Values.Count);
            Assert.IsTrue(tree.Values.ContainsKey("middleprop"));
            Assert.IsTrue(tree.Values.ContainsKey("lastprop"));

            Assert.AreEqual(2, tree.Children.Count);
            Assert.AreEqual(3, tree.Children[0].Values.Count);
            Assert.AreEqual(2, tree.Children[1].Values.Count);
        }

        [Test]
        public void ModuleManagerOne()
        {
            const string file = "..\\..\\testdata\\fail\\mmnode1.cfg";
            var parser = new Parser(true);
            var kc = parser.ParseConfig(file);

            Assert.AreEqual(1, kc.Count);

            var tree = kc.First();

            Assert.True(tree.Values.ContainsKey("name"));
            Assert.AreEqual("test", tree.Values["name"]);//.First());
        }

        [Test]
        public void ModuleManagerTwo()
        {
            const string file = "..\\..\\testdata\\fail\\mmnode2.cfg";
            var parser = new Parser(true);
            var kc = parser.ParseConfig(file);

            Assert.AreEqual(1, kc.Count);

            var tree = kc.First();

            Assert.True(tree.Values.ContainsKey("name"));
            Assert.AreEqual("test", tree.Values["name"]);//.First());
        }

        [Test]
        public void ModuleManagerThree()
        {
            const string file = "..\\..\\testdata\\fail\\mmnode3.cfg";
            var parser = new Parser(true);
            var kc = parser.ParseConfig(file);

            Assert.AreEqual(2, kc.Count);

            var tree = kc.First();
            var treetwo = kc[1];

            Assert.True(tree.Values.ContainsKey("name"));
            Assert.AreEqual("test", tree.Values["name"]);//.First());
            Assert.True(treetwo.Values.ContainsKey("name"));
            Assert.AreEqual("2ndtest", treetwo.Values["name"]);//.First());
        }

        [Test]
        public void ModuleManagerFour()
        {
            const string file = "..\\..\\testdata\\fail\\mmnode4.cfg";
            var parser = new Parser(true);
            var kc = parser.ParseConfig(file);

            Assert.AreEqual(3, kc.Count);

            var tree = kc.First();
            var treetwo = kc[1];
            var treethree = kc[2];

            Assert.True(tree.Values.ContainsKey("name"));
            Assert.AreEqual("test", tree.Values["name"]);//.First());
            Assert.True(treetwo.Values.ContainsKey("name"));
            Assert.AreEqual("2ndtest", treetwo.Values["name"]);//.First());
            Assert.True(treethree.Values.ContainsKey("name"));
            Assert.AreEqual("3rdtest", treethree.Values["name"]);//.First());
        }

        [Test]
        public void FilterTest()
        {
            const string file = "..\\..\\testdata\\filter.cfg";
            var filters = new List<string> { "PART", "ANOTHER" };
            var parser = new Parser(filters, false, true);
            var kc = parser.ParseConfig(file);


            Assert.AreEqual(2, kc.Count);

            var part = kc.First();
            Console.WriteLine(part);
            var another = kc[1];
            Console.WriteLine(another);

            Assert.True(part.Values.ContainsKey("name"));
            Assert.AreEqual("partname", part.Values["name"]);//.First());
            Assert.AreEqual(1, part.Children.Count);
            Assert.True(another.Values.ContainsKey("name"));
            Assert.AreEqual("foo", another.Values["name"]);//.First());
        }

        [Test]
        public void HeadlessFilterTest()
        {
            const string file = "..\\..\\testdata\\headlessfilter.cfg";
            var filters = new List<string> { "OTHER" };
            var parser = new Parser(filters, false, true);
            var kc = parser.ParseConfig(file);

            Assert.IsEmpty(kc, "Config is not empty!");

            var filters2 = new List<string> { "HEADLESSFILTER" };
            var parser2 = new Parser(filters2, false, true);
            var kc2 = parser2.ParseConfig(file);

            Assert.AreEqual(1, kc2.Count, $"Config contains {kc2.Count} nodes");

            var part = kc2.First();
            Console.WriteLine(part);

            Assert.True(part.Values.ContainsKey("name"), "part does not contain name");
            Assert.AreEqual("external", part.Values["name"], "Name is not \"external\"");
            //Assert.AreEqual("external", part.Values["name"].First(), "Name is not \"external\"");
            // TODO
            //Assert.AreEqual(1, part.Children.Count, $"Actual child count is {part.Children.Count}");
        }

        [Test]
        public void SkipBrokenNodesWithValidation()
        {
            const string file = "..\\..\\testdata\\fail\\invalidnodename.cfg";
            var parser = new Parser(true);
            var kc = parser.ParseConfig(file);

            Assert.AreEqual(1, kc.Count);

            var part = kc.First();
            Console.WriteLine(part);

            Assert.True(part.Values.ContainsKey("name"));
            Assert.AreEqual(24, part.Values.Count);
            Assert.AreEqual(0, part.Children.Count);
        }

        [Test]
        public void SkipBrokenNodesWithoutValidation()
        {
            const string file = "..\\..\\testdata\\fail\\invalidnodename.cfg";
            var parser = new Parser();
            var kc = parser.ParseConfig(file);

            Assert.AreEqual(1, kc.Count);

            var part = kc.First();
            Console.WriteLine(part);

            Assert.True(part.Values.ContainsKey("name"));
            Assert.AreEqual(24, part.Values.Count);
            Assert.AreEqual(1, part.Children.Count);
        }

        [Test]
        public void DuplicateBracketsSkipsNode()
        {
            const string file = "..\\..\\testdata\\fail\\duplicatebrackets.cfg";
            var parser = new Parser();
            var kc = parser.ParseConfig(file);

            Assert.AreEqual(1, kc.Count);

            var part = kc.First();
            Console.WriteLine(part);

            Assert.True(part.Values.ContainsKey("name"));
            Assert.AreEqual(3, part.Values.Count);
            Assert.AreEqual(2, part.Children.Count);

            var child1 = part.Children[0];
            var child2 = part.Children[1];

            Assert.AreEqual("RESOURCE", child1.Name);
            Assert.True(child1.Values.ContainsKey("name"));
            Assert.True(child1.Values.ContainsKey("amount"));
            Assert.AreEqual(3, child1.Values.Count);

            Assert.AreEqual("MODULE", child2.Name);
            Assert.True(child2.Values.ContainsKey("name"));
            Assert.AreEqual(2, child2.Values.Count);
        }
    }
}