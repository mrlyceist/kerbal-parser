using System;
using System.Collections.Generic;
using System.Linq;
using KerbalParser;
using NUnit.Framework;

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
				Assert.IsInstanceOf<IDictionary<string, List<string>>>
					(kerbalNode.Values);
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

	[TestFixture]
	public class FailureTests
	{
		[Test]
		[ExpectedException(
			typeof (ParseErrorException),
			ExpectedMessage = "matching bracket",
			MatchType = MessageMatch.Contains,
			Handler = "HandlerMethod")]
		public void MismatchBrackets()
		{
			const string file = "..\\..\\testdata\\fail\\mismatch.cfg";
			var parser = new Parser();
			parser.ParseConfig(file);
		}

		[Test]
		[ExpectedException(
			typeof (ParseErrorException),
			ExpectedMessage = "Invalid node name \"Part\"",
			MatchType = MessageMatch.Contains,
			Handler = "HandlerMethod")]
		public void InvalidNodeName()
		{
			const string file = "..\\..\\testdata\\fail\\invalidnodename.cfg";
			var parser = new Parser();
			parser.ParseConfig(file);
		}

		[Test]
		[ExpectedException(
			typeof (ParseErrorException),
			ExpectedMessage = "Invalid node name \"}",
			MatchType = MessageMatch.Contains,
			Handler = "HandlerMethod")]
		public void DuplicateBrackets()
		{
			const string file = "..\\..\\testdata\\fail\\duplicatebrackets.cfg";
			var parser = new Parser();
			parser.ParseConfig(file);
		}

		public void HandlerMethod(Exception e)
		{
			Console.WriteLine(e.Message);
		}
	}

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

			Assert.AreEqual(4, tree.Values.Count);
			Assert.IsTrue(tree.Values.ContainsKey("name"));
			Assert.IsTrue(tree.Values.ContainsKey("module"));
			Assert.IsTrue(tree.Values.ContainsKey("author"));
			Assert.IsTrue(tree.Values.ContainsKey("description"));
			Assert.AreEqual(3, tree.Values["name"].Count);
			Assert.AreEqual(2, tree.Values["author"].Count);
			Assert.IsTrue(tree.Values["name"].First() == "batteryBankMini");
			Assert.IsTrue(tree.Values["name"][1] == "batteryBankMini2");
			Assert.IsTrue(tree.Values["name"][2] == "spice");
			Assert.IsTrue(tree.Values["author"].First() == "Squad");
			Assert.IsTrue(tree.Values["author"][1] == "Bob");
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
			var parser = new Parser();
			var kc = parser.ParseConfig(file);

			Assert.AreEqual(1, kc.Count);

			var tree = kc.First();

			Assert.True(tree.Values.ContainsKey("name"));
			Assert.AreEqual("test", tree.Values["name"].First());
		}

		[Test]
		public void ModuleManagerTwo()
		{
			const string file = "..\\..\\testdata\\fail\\mmnode2.cfg";
			var parser = new Parser();
			var kc = parser.ParseConfig(file);

			Assert.AreEqual(1, kc.Count);

			var tree = kc.First();

			Assert.True(tree.Values.ContainsKey("name"));
			Assert.AreEqual("test", tree.Values["name"].First());
		}

		[Test]
		public void ModuleManagerThree()
		{
			const string file = "..\\..\\testdata\\fail\\mmnode3.cfg";
			var parser = new Parser();
			var kc = parser.ParseConfig(file);

			Assert.AreEqual(2, kc.Count);

			var tree = kc.First();
			var treetwo = kc[1];

			Assert.True(tree.Values.ContainsKey("name"));
			Assert.AreEqual("test", tree.Values["name"].First());
			Assert.True(treetwo.Values.ContainsKey("name"));
			Assert.AreEqual("2ndtest", treetwo.Values["name"].First());
		}

		[Test]
		public void ModuleManagerFour()
		{
			const string file = "..\\..\\testdata\\fail\\mmnode4.cfg";
			var parser = new Parser();
			var kc = parser.ParseConfig(file);

			Assert.AreEqual(3, kc.Count);

			var tree = kc.First();
			var treetwo = kc[1];
			var treethree = kc[2];

			Assert.True(tree.Values.ContainsKey("name"));
			Assert.AreEqual("test", tree.Values["name"].First());
			Assert.True(treetwo.Values.ContainsKey("name"));
			Assert.AreEqual("2ndtest", treetwo.Values["name"].First());
			Assert.True(treethree.Values.ContainsKey("name"));
			Assert.AreEqual("3rdtest", treethree.Values["name"].First());
		}
	}
}
