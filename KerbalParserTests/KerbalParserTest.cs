using System.Collections.Generic;
using KerbalParser;
using NUnit.Framework;

namespace KerbalParserTests
{
	[TestFixture]
	internal class KerbalParserTest
	{
		private IList<KerbalNode> _kerbalTree;

		[SetUp]
		public void Init()
		{
			const string file = "..\\..\\testdata\\simple.cfg";
			_kerbalTree = Parser.ParseConfig(file);
		}

		[TearDown]
		public void CleanUp()
		{
			_kerbalTree = null;
		}

		[Test]
		public void CreateParser()
		{
			Assert.IsInstanceOf<IList<KerbalNode>>(_kerbalTree);
		}

		[Test]
		public void GenericParse()
		{
			Assert.IsNotEmpty(_kerbalTree);

			foreach (var kerbalNode in _kerbalTree)
			{
				Assert.IsNotNullOrEmpty
					(kerbalNode.Name);
				Assert.IsInstanceOf<IDictionary<string, string>>
					(kerbalNode.Values);
				Assert.IsInstanceOf<IList<KerbalNode>>
					(kerbalNode.Children);
			}
		}
	}
}
