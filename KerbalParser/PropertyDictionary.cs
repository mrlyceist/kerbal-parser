using System.Collections.Generic;

namespace KerbalParser
{
    public class PropertyDictionary<TKey, TValue> : Dictionary<TKey, TValue>
    {
        public new TValue this[TKey key]
        {
            get {
                try
                {
                    return base[key];
                }
                catch (KeyNotFoundException )
                {
                    throw new ParseErrorException($"Property not found: {key}");
                }
            }
            set {
                try
                {
                    base[key] = value;
                }
                catch (KeyNotFoundException)
                {
                    throw new ParseErrorException($"Property not found: {key}");
                }
            }
        }
    }
}