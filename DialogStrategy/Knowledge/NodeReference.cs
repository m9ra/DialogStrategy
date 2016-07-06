using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DialogStrategy.Knowledge
{
    public class NodeReference
    {
        public readonly object Data;

        internal NodeReference(object data)
        {
            Data = data;
        }

        public override string ToString()
        {
            return "[Ref: " + Data.ToString() + "]";
        }

        public override bool Equals(object obj)
        {
            var o = obj as NodeReference;
            if (o == null)
                return Data == obj;

            return Data.Equals(o.Data);
        }

        public override int GetHashCode()
        {
            return Data.GetHashCode();
        }
    }
}
