using System.Collections;
using System.Collections.Generic;
using KerbalParser;
using NUnit.Framework;

namespace KerbalParserTests
{
	[TestFixture]
	internal class KerbalParserTest
	{
		[Test]
		public void CreateParser()
		{
			const string file = "Hello World";
			var kp = KerbalParser.KerbalParser.ParseConfig(file);

			Assert.IsInstanceOf<IList<KerbalNode>>(kp);
		}

		[Test]
		public void GenericParse()
		{
			const string file = "testdata\\simple.cfg";

			var kp = KerbalParser.KerbalParser.ParseConfig(file);

			Assert.IsNotEmpty(kp);

			foreach (var kerbalNode in kp)
			{
				Assert.IsNotNullOrEmpty(kerbalNode.Name);
				Assert.IsInstanceOf<IDictionary<string, string>>(kerbalNode.Values);
				Assert.IsInstanceOf<IList<KerbalNode>>(kerbalNode.Children);
			}
		}
	}
}
