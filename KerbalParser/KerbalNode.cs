using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace KerbalParser
{
	[DataContract]
	public class KerbalNode
	{
		public KerbalNode(string name, KerbalNode parent = null)
		{
			Name = name;
			Parent = parent;
		}

		[DataMember]
		public string Name { get; set; }

		[DataMember]
		public KerbalNode Parent { get; set; }

		public IDictionary<string, string> Values
		{
			get { throw new NotImplementedException(); }
		}

		public IList<KerbalNode> Children
		{
			get { throw new NotImplementedException(); }
		}

		public override string ToString()
		{
			return JsonSerializer.To(this);
		}
	}
}