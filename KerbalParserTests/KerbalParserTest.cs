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

			Assert.IsTrue(
			              node.Values.Count == 24,
			              "Incorrect number of values. Expected 24, Was : " +
			              node.Values.Count
				);
			Assert.IsTrue(
			              node.Children.Count == 2,
			              "Incorrect number of children. Expected 2, Was : " +
			              node.Children.Count
				);

			var childOne = node.Children[0];
			var childTwo = node.Children[1];

			Console.Write(childOne);

			Assert.IsTrue(
			              childOne.Values.Count == 3,
			              "Incorrect number of values in child. Expected 3, " +
			              "was " + childOne.Values.Count
				);

			Console.Write(childTwo);

			Assert.IsTrue(
			              childTwo.Values.Count == 2,
			              "Incorrect number of values in child. Expected 2, " +
			              "was " + childTwo.Values.Count
				);
		}
	}
}
