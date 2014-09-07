using System;
using System.Collections.Generic;
using KerbalParser;
using NUnit.Framework;

namespace KerbalParserTests
{
	[TestFixture]
	internal class KerbalParserTest
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
				Assert.IsInstanceOf<IDictionary<string, string>>
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
			typeof (Exception),
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
			typeof(Exception),
			ExpectedMessage = "Invalid node name",
			MatchType = MessageMatch.Contains,
			Handler = "HandlerMethod")]
		public void InvalidNodeName()
		{
			const string file = "..\\..\\testdata\\fail\\invalidnodename.cfg";
			var parser = new Parser();
			parser.ParseConfig(file);
		}

		public void HandlerMethod(Exception e)
		{
			Console.WriteLine(e.Message);
		}
	}
}
