using System;
using System.Collections.Generic;

namespace KerbalParser
{
	public class KerbalParser
	{
		public static IList<KerbalNode> ParseConfig(String configFile)
		{
			IList<KerbalNode> kerbalNodes = new List<KerbalNode>();

			return kerbalNodes;
		}
	}

	public class KerbalNode
	{
		public KerbalNode(string name)
		{
			Name = name;
		}

		public string Name { get; set; }

		public IDictionary<string, string> Values
		{
			get { throw new NotImplementedException(); }
		}

		public IList<KerbalNode> Children
		{
			get { throw new NotImplementedException(); }
		}
	}
}
