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
            Values = new PropertyDictionary<string, string>();
            //Values = new Dictionary<string, string>();
            Children = new List<KerbalNode>();

            Parent?.Children.Add(this);
        }

        [DataMember(Order = 1)]
        public string Name { get; set; }

        public KerbalNode Parent { get; set; }

        [DataMember(Order = 3)]
        public PropertyDictionary<string, string> Values { get; set; }
        //public IDictionary<string, string> Values { get; set; }

        [DataMember(Order = 4)]
        public IList<KerbalNode> Children { get; set; }

        public override string ToString()
        {
            return JsonSerializer.To(this);
        }
    }
}
